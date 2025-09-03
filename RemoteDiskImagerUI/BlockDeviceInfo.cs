using System.Text.Json;
using System.Text.Json.Serialization;

namespace RemoteDiskImanger;

public class BlockDeviceInfo {
    /// <summary>
    /// Converts nullable long values to and from JSON.
    /// </summary>
    /// <remarks>Had some problems with null JSON values thus this manuell converter</remarks>
    private class NullableLongConverter : JsonConverter<long?> {
        public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            switch (reader.TokenType) {
                case JsonTokenType.None:
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    string strValue = reader.GetString() ?? "";
                    return long.TryParse(strValue, out long parsedValue) ? parsedValue : null;
                case JsonTokenType.Number:
                    return reader.TryGetInt64(out long longValue) ? longValue : null;
                default:
                    throw new JsonException($"Unexpected token parsing long. Expected Number or Null, got {reader.TokenType}.");
            }
        }
        public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options) {
            if (value.HasValue) {
                writer.WriteNumberValue(value.Value);
            } else {
                writer.WriteNullValue();
            }
        }
    }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("size")]
    public long Size { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    [JsonPropertyName("fstype")]
    public string? FileSystemType { get; set; } = "";
    [JsonPropertyName("children")]
    public BlockDeviceInfo[]? Children { get; set; }
    [JsonPropertyName("fssize"), JsonConverter(typeof(NullableLongConverter))]
    public long? FileSystemSize { get; set; } = null;
    [JsonPropertyName("fsused"), JsonConverter(typeof(NullableLongConverter))]
    public long? FileSystemUsed { get; set; } = null;
    [JsonPropertyName("path")]
    public string Path { get; set; } = "";
    [JsonPropertyName("rm")]
    public bool IsRemovable { get; set; }
    [JsonPropertyName("ro")]
    public bool IsReadOnly { get; set; }

    [JsonIgnore]
    public string HumanReadableSize => (this.FileSystemSize ?? this.Size).ToHumanReadableSize(this.FileSystemUsed);

    public IEnumerable<BlockDeviceInfo> AllDevices() {
        yield return this;
        if (this.Children is not null) {
            foreach (var child in this.Children) {
                foreach (var desc in child.AllDevices()) {
                    yield return desc;
                }
            }
        }
    }
}

public static class BlockDeviceInfoExtensions {
    private const string INITIAL_INDENT = "";
    private const string ADDITIONAL_INDENT = "  ";

    private static string SanatizeFileSystemType(string? filesystemType) {
        if (filesystemType is null) return "";
        if (filesystemType == "linux_raid_member") return "raid";
        return filesystemType;
    }

    private static (List<string[]> Values, int[] Widths) GetWidths(IEnumerable<BlockDeviceInfo> infos, string indent) {
        int[] widths = HEADERS.Select(h => h.Length).ToArray();
        List<string[]> rows = new();
        foreach (var info in infos) {
            string[] row = {
                indent + info.Path,
                info.HumanReadableSize,
                info.Type,
                SanatizeFileSystemType(info.FileSystemType),
            };
            for (int i = 0; i < row.Length; i++) {
                widths[i] = int.Max(widths[i], row[i].Length);
            }
            rows.Add(row);
            if (info.Children is not null) {
                string childIndent = ADDITIONAL_INDENT + indent;
                (List<string[]> childRows, int[] childWidths) = GetWidths(info.Children, childIndent);
                rows.AddRange(childRows);
                for (int i = 0; i < widths.Length; i++) {
                    widths[i] = int.Max(widths[i], childWidths[i]);
                }
            }
        }
        return (rows, widths);
    }

    private static void PrintLine(string[] row, int[] widths, int divider) {
        for (int i = 0; i < row.Length; i++) {
            Console.Write(row[i].PadRight(widths[i] + divider));
        }
        Console.WriteLine();
    }

    private static readonly string[] HEADERS = new string[] { "NAME", "SIZE", "TYPE", "FSTYPE" };

    public static void Print(this IEnumerable<BlockDeviceInfo> infos, int divider = 2) {
        (List<string[]> rows, int[] widths) = GetWidths(infos, INITIAL_INDENT);
        PrintLine(HEADERS, widths, divider);
        foreach (string[] row in rows) {
            PrintLine(row, widths, divider);
        }
    }

    public static void PrintNumbered(this List<BlockDeviceInfo> infos, int divider = 2) {
        int numberWidth = infos.Count.ToString().Length + 1 + divider;
                
        (List<string[]> rows, int[] widths) = GetWidths(infos, INITIAL_INDENT);

        // Print header
        Console.Write("".PadRight(numberWidth));
        PrintLine(HEADERS, widths, divider);

        // Print device infos
        for (int i = 0; i < rows.Count; i++) {
            Console.Write($"{i}:".PadRight(numberWidth));
            PrintLine(rows[i], widths, divider);
        }
    }

    private static readonly string[] SizeSuffixes = { "", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string ToHumanReadableSize(this long bytes, long? used = null) {
        int suffixIndex = 0;
        double size = bytes;
        while (size > 1024.0 && suffixIndex + 1 < SizeSuffixes.Length) {
            size /= 1024.0;
            suffixIndex++;
        }

        string result;

        if (size > 100.0)
            result = $"{size:0}{SizeSuffixes[suffixIndex]}";
        else if (size > 10.0)
            result = $"{size:0.0}{SizeSuffixes[suffixIndex]}";
        else
            result = $"{size:0.00}{SizeSuffixes[suffixIndex]}";

        if (used.HasValue) {
            double usedPercent = (double)used.Value / (double)bytes * 100.0;
            result += $" ({usedPercent:0.0}% used)";
        }

        return result;
    }
}
