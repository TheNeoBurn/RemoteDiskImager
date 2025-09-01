using System.Text;

namespace RemoteDiskImanger;

public static class ConsoleHelper {
    public static void WriteLine(string message, ConsoleColor foreColor, ConsoleColor? backColor = null) {
        ConsoleColor currForeColor = Console.ForegroundColor;
        ConsoleColor currBackColor = Console.BackgroundColor;
        Console.ForegroundColor = foreColor;
        if (backColor.HasValue) Console.BackgroundColor = backColor.Value;
        Console.WriteLine(message);
        Console.ForegroundColor = currForeColor;
        if (backColor.HasValue) Console.BackgroundColor = currBackColor;
    }
    public static void Write(string message, ConsoleColor foreColor, ConsoleColor? backColor = null) {
        ConsoleColor currForeColor = Console.ForegroundColor;
        ConsoleColor currBackColor = Console.BackgroundColor;
        Console.ForegroundColor = foreColor;
        if (backColor.HasValue) Console.BackgroundColor = backColor.Value;
        Console.Write(message);
        Console.ForegroundColor = currForeColor;
        if (backColor.HasValue) Console.BackgroundColor = currBackColor;
    }

    public static void WriteLineInfo(string message) {
        WriteLine(message, ConsoleColor.White, ConsoleColor.Black);
    }

    public static void WriteLineWarn(string message) {
        WriteLine(message, ConsoleColor.Yellow, ConsoleColor.Black);
    }

    public static void WriteLineError(string message) {
        WriteLine(message, ConsoleColor.Red, ConsoleColor.Black);
    }


    public static string ReadHiddenLine(char? passwordChar = '*') {
        var password = new StringBuilder();
        ConsoleKeyInfo key;
        while (true) {
            key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
                break;
            if (key.Key == ConsoleKey.Backspace && password.Length > 0) {
                password.Length--;
                if (passwordChar.HasValue) {
                    Console.Write(key.KeyChar);
                    Console.Write(' ');
                    Console.Write(key.KeyChar);
                }
            } else if (!char.IsControl(key.KeyChar)) {
                password.Append(key.KeyChar);
                if (passwordChar.HasValue) Console.Write(passwordChar.Value);
            }
        }
        Console.WriteLine();
        return password.ToString();
    }

    public static void WriteLastLine(string text) {
        int currentLineCursor = Console.CursorTop;
        if (currentLineCursor > 0)
            Console.SetCursorPosition(0, currentLineCursor - 1);
        Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
        Console.SetCursorPosition(0, currentLineCursor - 1);
        Console.WriteLine(text);
    }
}
