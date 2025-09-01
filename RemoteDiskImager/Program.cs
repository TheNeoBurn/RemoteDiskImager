using RemoteDiskImanger;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Diagnostics;
using System.Text.RegularExpressions;

Regex REX_Server = new(@"^(?<user>[^@]+)@(?<host>\[[0-9a-fA-F:]+\]|[^:]+)(:(?<path>.+))?$", RegexOptions.Compiled);
Regex REX_BlockSize = new(@"^\d+[KMG]?$", RegexOptions.Compiled);
Regex REX_Number = new(@"^\d+$", RegexOptions.Compiled);

string host = "";
int port = 22;
string username = "";
string password = "";
string path = "";
Exception? error = null;
bool showHelp = false;
string blockSize = "8M";
string? count = null;
bool sourceDone = false;
FileInfo? targetFile = null;
FileInfo? certFile = null;
SuCommandType suCommand = SuCommandType.None;
string? suPassword = null;

#region Parse arguments
try {
    for (int i = 0; i < args.Length; i++) {
        string arg = args[i];
        switch (arg) {
            case "--port":
            case "-p":
                i++;
                if (i >= args.Length) throw new Exception("Port is number missing!");
                if (!ushort.TryParse(args[i], out ushort p)) throw new Exception("Invalid port number!");
                port = p;
                break;
            case "--password":
            case "-P":
                i++;
                if (i >= args.Length) throw new Exception("Password missing");
                password = args[i];
                break;
            case "--block-size":
            case "-b":
                i++;
                if (i >= args.Length) throw new Exception("Block size missing!");
                if (!REX_BlockSize.IsMatch(args[i])) throw new Exception("Invalid block size!");
                blockSize = args[i];
                break;
            case "--count":
            case "-c":
                i++;
                if (i >= args.Length) throw new Exception("Count missing!");
                if (!REX_BlockSize.IsMatch(args[i])) throw new Exception("Invalid block size!");
                count = args[i];
                break;
            case "--certificate":
            case "--cert":
            case "-I":
                i++;
                if (i >= args.Length) throw new Exception("Certificate file missing!");
                certFile = new FileInfo(args[i]);
                if (!certFile.Exists) throw new Exception("Certificate file not found!");
                break;
            case "--su":
            case "-s":
                suCommand = SuCommandType.Su;
                break;
            case "--sudo":
            case "-d":
                suCommand = SuCommandType.Sudo;
                break;
            case "--su-password":
            case "-S":
                i++;
                if (i >= args.Length) throw new Exception("su/sudo password missing!");
                suPassword = args[i];
                break;
            case "--help":
            case "-h":
            case "-?":
                showHelp = true;
                break;
            default:
                if (!sourceDone) {
                    Match match = REX_Server.Match(arg);
                    if (!match.Success) throw new Exception("Invalid source format!");
                    username = match.Groups["user"].Value;
                    host = match.Groups["host"].Value;
                    if (match.Groups["path"].Success) path = match.Groups["path"].Value;
                    sourceDone = true;
                } else if (targetFile is null) {
                    targetFile = new FileInfo(arg);
                }  else {
                    throw new Exception($"Invalid argument \"{arg}\"!");
                }
                break;
        }
    }
} catch (Exception ex) {
    error = ex;
    showHelp = true;
}
#endregion

#region Show help if needed
if (showHelp || string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || targetFile is null) {
    if (error is not null) {
        ConsoleHelper.WriteLineError("Error: " + error.Message);
        Console.WriteLine();
    }

    Console.WriteLine("RemodeDiskImager");
    Console.WriteLine();
    Console.WriteLine("  A tool to image remote disks over SSH.");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  RemoteDiskImager [options] <user>@<host>[:<path>] <target>");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -p, --port <port>            SSH port number (default: 22)");
    Console.WriteLine("  -P, --password <password>    SSH password");
    Console.WriteLine("  -I, --cert <file>            SSH server certificate file");
    Console.WriteLine("  -s, --su                     Use 'su' to gain root privilege");
    Console.WriteLine("  -d, --sudo                   Use 'sudo -s' to gain root privilege");
    Console.WriteLine("  -S, --su-password <password> Password for 'su' or 'sudo'");
    Console.WriteLine("  -b, --block-size <size>      Block size (default: 8M)");
    Console.WriteLine("  -c, --count <count>          Number of blocks to copy (default: all)");
    Console.WriteLine("  -h, -?, --help               Show this help message");
    Console.WriteLine();

    return 0;
}
#endregion


// Link cancel event to async token
bool isCanceled = false;
CancellationTokenSource cancelSource = new();
Console.CancelKeyPress += (s, e) => {
    e.Cancel = isCanceled;
    isCanceled = true;
    cancelSource.Cancel();
};

#region Create Connection
SshClient client;
if (certFile is not null) {
    int retry = 0;
    PrivateKeyFile cert;
RETRY_POINT:
    try {
        cert = new(certFile.FullName, password);
    } catch (SshPassPhraseNullOrEmptyException ex) {
        if (retry != 0) ConsoleHelper.WriteLineError("Error: Invalid passphrase!");
        if (++retry > 3) {
            Console.WriteLine();
            return 1;
        }
        Console.Write("Certificate passphrase: ");
        try {
            password = ConsoleHelper.ReadHiddenLine(null) ?? throw new Exception("Passphrase input error!");
        } catch (Exception) {
            ConsoleHelper.WriteLineError("Error: Passphrase input error!");
            Console.WriteLine();
            return 1;
        }
        goto RETRY_POINT;
    } catch (Exception ex) {
        ConsoleHelper.WriteLineError("Error: Unable to load certificate file!" + ex.Message);
        Console.WriteLine();
        return 1;
    }

    client = new SshClient(new ConnectionInfo(host, port, username, new PrivateKeyAuthenticationMethod(username, cert)));
} else {
    if (string.IsNullOrEmpty(password)) {
        Console.Write($"Password for {username}@{host}: ");
        try {
            password = ConsoleHelper.ReadHiddenLine(null) ?? throw new Exception("Password input error!");
        } catch (Exception) {
            ConsoleHelper.WriteLineError("Error: Password input error!");
            Console.WriteLine();
            return 1;
        }
    }

    client = new SshClient(host, port, username, password);
}
#endregion

#region Let user select su/sudo password if needed
if (suCommand != SuCommandType.None && suPassword is null) {
    Console.Write("su/sudo password: ");
    try {
        suPassword = ConsoleHelper.ReadHiddenLine(null) ?? throw new Exception("su/sudo password input error!");
    } catch (Exception) {
        ConsoleHelper.WriteLineError("Error: Passphrase input error!");
        Console.WriteLine();
        return 1;
    }
}
#endregion

try {
    await client.ConnectAsync(cancelSource.Token);
    Console.WriteLine("Connected.");

    Console.WriteLine("Retrieving block device information...");
    List<BlockDeviceInfo> deviceInfos = await client.GetBlockDeviceInfosAsync(suCommand, suPassword, cancelSource.Token);
    List<BlockDeviceInfo> flatDeviceInfos = deviceInfos.SelectMany(d => d.AllDevices()).ToList();

    #region Let user select device path if not specified
    while (string.IsNullOrEmpty(path)) {
        try {
            Console.WriteLine();
            deviceInfos.PrintNumbered();
            Console.WriteLine();
            Console.Write("Enter device path or number: ");
            path = Console.ReadLine() ?? "";
            if (REX_Number.IsMatch(path)) {
                int index = int.Parse(path);
                if (index < 0 || index >= flatDeviceInfos.Count) {
                    ConsoleHelper.WriteLineError("Error: Invalid device number!");
                    continue;
                }
                path = flatDeviceInfos[index].Path ?? flatDeviceInfos[index].Name;
            }
        } catch (Exception ex) {
            ConsoleHelper.WriteLineError("Error: " + ex.Message);
            Console.WriteLine();
            return 1;
        }
    }
    #endregion

    BlockDeviceInfo? selectedDevice = flatDeviceInfos.FirstOrDefault(d => d.Path == path || d.Name == path);

    Console.WriteLine();
    Console.WriteLine($"Source: {username}@{host}:{path}");
    Console.WriteLine($"Target: {targetFile.FullName}");
    if (selectedDevice is not null) {
        Console.WriteLine($"Size:   {selectedDevice.Size.ToHumanReadableSize()} ({selectedDevice.Size} bytes)");
    }
    Console.WriteLine();


    object locker = new();
    Stopwatch stopwatch = Stopwatch.StartNew();
    (long BytesDone, long? BytesFull) progress = (0, null);

    using (FileStream targetStream = targetFile.Open(FileMode.Create, FileAccess.Write, FileShare.None)) {
        Console.WriteLine("Starting copying data...");
        Stopwatch progressTimer = Stopwatch.StartNew();
        char[] progressChars = new char[] { '|', '/', '-', '\\' };
        int progressSign = 0;
        Task task = client.ReadBlockDeviceAsync(
            suCommand, 
            suPassword, 
            selectedDevice?.Path ?? path, 
            blockSize, 
            count, 
            selectedDevice?.Size, 
            targetStream, 
            (bytesDone, bytesFull) => {
                lock (locker) {
                    progress.BytesDone = bytesDone;
                    progress.BytesFull = bytesFull;
                }

                if (!stopwatch.IsRunning) stopwatch.Start();
            }, 
            cancelSource.Token
        );
        long bytesDone;
        long? bytesFull;
        while (!(task.IsCompleted || task.IsCanceled)) {
            await Task.Delay(200, cancelSource.Token);
            lock (locker) {
                bytesDone = progress.BytesDone;
                bytesFull = progress.BytesFull ?? -1;
            }
            if (progressTimer.ElapsedMilliseconds > 250) {
                progressSign = (progressSign + 1) % progressChars.Length;
                progressTimer.Restart();
            }
            string speed = "";
            if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds > 1000) {
                long bps = Convert.ToInt64(Math.Floor(bytesDone / ((double)stopwatch.ElapsedMilliseconds / 1000.0)));
                speed = $" ({bps.ToHumanReadableSize()}/s)";
            }
            string progressText;
            if (bytesFull.HasValue) {
                double percent = (double)bytesDone / (double)bytesFull.Value * 100.0;
                progressText = $"{bytesDone.ToHumanReadableSize()} / {bytesFull.Value.ToHumanReadableSize()} ({percent:0.00}%)";
            } else {
                progressText = $"{bytesDone} ({bytesDone.ToHumanReadableSize()})";
            }
            ConsoleHelper.WriteLastLine($"Copying...{progressChars[progressSign]} {progressText}{speed}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Done.");
} catch (Exception ex) {
    ConsoleHelper.WriteLineError("Error: " + ex.Message);
    Console.WriteLine();
    return 1;
} finally {
    try { if (client.IsConnected) client.Disconnect(); } catch { }
}


return 0;