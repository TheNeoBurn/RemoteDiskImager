using Renci.SshNet;
using System.Text;
using System.Text.Json;

namespace RemoteDiskImanger;

public static class SshCommandHelper {
    public static long ParseByteDef(string value) {
        long bytes = long.Parse(value.TrimEnd(['K', 'M', 'G']));
        if (value.EndsWith('K')) bytes *= 1024;
        else if (value.EndsWith('M')) bytes *= 1024 * 1024;
        else if (value.EndsWith('G')) bytes *= 1024 * 1024 * 1024;
        return bytes;
    }

    public static async Task<List<BlockDeviceInfo>> GetBlockDeviceInfosAsync(this SshClient client, SuCommandType suType, string? suPassword, CancellationToken cancelToken) {
        const int BLK_BUFFER_SIZE = 65536;
        const string EXECUTION_AUTHENTICATION_FAILURE = "EXECUTION_AUTHENTICATION_FAILURE";

        string cmdText = "lsblk -Jbf -o FSSIZE,FSTYPE,FSUSED,LABEL,NAME,PARTTYPE,PATH,RM,RO,SIZE,TYPE";
        if (suType != SuCommandType.None) {
            switch (suType) {
                case SuCommandType.Sudo:
                    cmdText = $"sudo {cmdText} || echo \"{EXECUTION_AUTHENTICATION_FAILURE}\"";
                    break;
                case SuCommandType.Su:
                    cmdText = $"su -c '{cmdText}' || echo \"{EXECUTION_AUTHENTICATION_FAILURE}\"";
                    break;
            }
        }

        SshCommand cmd = client.CreateCommand(cmdText);
        Task task = cmd.ExecuteAsync(cancelToken);

        using Stream inpStream = cmd.CreateInputStream();
        using MemoryStream dataStream = new();
        using MemoryStream extStream = new();
        byte[] buffer = new byte[BLK_BUFFER_SIZE];
        int available;
        int bytesRead;
        int doneCount = 0;
        bool isPasswordSent = false;

        while (true) {
            available = (int)long.Min(BLK_BUFFER_SIZE, (cmd.ExtendedOutputStream.Length - cmd.ExtendedOutputStream.Position));
            if (available > 0) {
                bytesRead = await cmd.ExtendedOutputStream.ReadAsync(buffer, 0, available, cancelToken);
                await extStream.WriteAsync(buffer, 0, bytesRead, cancelToken);

                // Check for password prompt
                if (!isPasswordSent && Encoding.UTF8.GetString(extStream.ToArray()).Contains("password", StringComparison.OrdinalIgnoreCase)) {
                    // Reset extended buffer stream
                    extStream.SetLength(0);

                    // Send password
                    await inpStream.WriteAsync(Encoding.UTF8.GetBytes((suPassword ?? "") + "\n"), cancelToken);
                    await inpStream.FlushAsync(cancelToken);

                    isPasswordSent = true;
                }
                doneCount = 0;
                continue;
            }

            available = (int)long.Min(BLK_BUFFER_SIZE, (cmd.OutputStream.Length - cmd.OutputStream.Position));
            if (available > 0) {
                bytesRead = await cmd.OutputStream.ReadAsync(buffer, 0, available, cancelToken);
                await dataStream.WriteAsync(buffer, 0, bytesRead, cancelToken);
                doneCount = 0;
                continue;
            }

            await Task.Delay(1, cancelToken);

            if (task.IsCompleted || task.IsCanceled) doneCount++;
            if (doneCount > 100) break;
        }

        if (dataStream.Length == 0) {
            return new List<BlockDeviceInfo>();
        }

        if (dataStream.Length < 1024) {
            if (Encoding.UTF8.GetString(dataStream.ToArray()).Contains(EXECUTION_AUTHENTICATION_FAILURE)) {
                string errMsg = "Failed to elevate privileges using su/sudo: Authentication failure!";
                if (extStream.Length > 0) {
                    errMsg = "Failed to elevate privileges using su/sudo: " + Encoding.UTF8.GetString(extStream.ToArray());
                }

                throw new Exception(errMsg);
            }
        }

       dataStream.Position = 0;
       JsonDocument doc = await JsonDocument.ParseAsync(dataStream, cancellationToken: cancelToken);

        List<BlockDeviceInfo> devices;
        try {
            devices = doc
                .RootElement
                .GetProperty("blockdevices")
                .Deserialize<List<BlockDeviceInfo>>() ?? throw new Exception("Unable to parse lsblk output to get block device info!");
        } catch (JsonException ex) {
            dataStream.Position = 0;
            string json = Encoding.UTF8.GetString(dataStream.ToArray());
            throw new Exception("Unable to parse lsblk output to get block device info!\n" + json, ex);
        }

        return devices;
    }

    public static async Task ReadBlockDeviceAsync(this SshClient client, SuCommandType suType, string? suPassword, string blockDevice, string blockSize, string? count, long? presumedByLength, Stream target, Action<long, long?>? onProgress, CancellationToken cancelToken) {
        const string EXECUTION_AUTHENTICATION_FAILURE = "EXECUTION_AUTHENTICATION_FAILURE";

        // Calculate block size in bytes
        long blockSizeBytes = ParseByteDef(blockSize);

        if (!presumedByLength.HasValue && !string.IsNullOrEmpty(count))
            presumedByLength = blockSizeBytes * ParseByteDef(count);

        // Escape double quotes in path
        blockDevice = blockDevice.Replace("\"", "\\\"").Replace("\"", "\\\"");

        // Send command to read block device
        string cmdText = $"dd if=\"{blockDevice}\" bs={blockSize} status=none";
        if (!string.IsNullOrEmpty(count)) cmdText += $" count={count}";
        
        if (suType != SuCommandType.None) {
            switch (suType) {
                case SuCommandType.Sudo:
                    cmdText = $"sudo {cmdText} || echo \"{EXECUTION_AUTHENTICATION_FAILURE}\"";
                    break;
                case SuCommandType.Su:
                    cmdText = $"su -c '{cmdText}' || echo \"{EXECUTION_AUTHENTICATION_FAILURE}\"";
                    break;
                case SuCommandType.None:
                    cmdText = $"{cmdText} || echo \"{EXECUTION_AUTHENTICATION_FAILURE}\"";
                    break;
            }
        }

        SshCommand cmd = client.CreateCommand(cmdText);
        Task task = cmd.ExecuteAsync(cancelToken);

        using Stream inpStream = cmd.CreateInputStream();
        using MemoryStream extStream = new();
        RollingBuffer lastBuffer = new RollingBuffer(64);
        byte[] buffer = new byte[blockSizeBytes];
        int available;
        int bytesRead = 0;
        int doneCount = 0;
        bool isPasswordSent = false;
        long totalRead = 0;

        while (true) {
            available = (int)long.Min(blockSizeBytes, (cmd.ExtendedOutputStream.Length - cmd.ExtendedOutputStream.Position));
            if (available > 0) {
                bytesRead = await cmd.ExtendedOutputStream.ReadAsync(buffer, 0, available, cancelToken);
                await extStream.WriteAsync(buffer, 0, bytesRead, cancelToken);

                // Check for password prompt
                if (!isPasswordSent && Encoding.UTF8.GetString(extStream.ToArray()).Contains("password", StringComparison.OrdinalIgnoreCase)) {
                    // Reset extended buffer stream
                    extStream.SetLength(0);

                    // Send password
                    await inpStream.WriteAsync(Encoding.UTF8.GetBytes((suPassword ?? "") + "\n"), cancelToken);
                    await inpStream.FlushAsync(cancelToken);

                    isPasswordSent = true;
                }
                doneCount = 0;
                continue;
            }

            available = (int)long.Min(blockSizeBytes, (cmd.OutputStream.Length - cmd.OutputStream.Position));
            if (available > 0) {
                bytesRead = await cmd.OutputStream.ReadAsync(buffer, 0, available, cancelToken);
                await target.WriteAsync(buffer, 0, bytesRead, cancelToken);
                lastBuffer.Append(buffer, 0, bytesRead);
                totalRead += bytesRead;
                onProgress?.Invoke(totalRead, presumedByLength);
                doneCount = 0;
                continue;
            }

            await Task.Delay(1, cancelToken);

            if (task.IsCompleted || task.IsCanceled) doneCount++;
            if (doneCount > 100) break;
        }

        if (lastBuffer.Received < 1024) {
            if (Encoding.ASCII.GetString(lastBuffer.Data).Contains(EXECUTION_AUTHENTICATION_FAILURE)) {
                string errMsg = "Failed to elevate privileges using su/sudo: Authentication failure!";
                if (extStream.Length > 0) {
                    errMsg = "Failed to elevate privileges using su/sudo: " + Encoding.UTF8.GetString(extStream.ToArray());
                }

                throw new Exception(errMsg);
            }
        }

        if (extStream.Length > 0) {
            string errMsg = "Failed to elevate privileges using su/sudo: " + Encoding.UTF8.GetString(extStream.ToArray());
            throw new Exception(errMsg);
        }
    }

}
