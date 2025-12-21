using System.Drawing;
using System.Text.RegularExpressions;

namespace ConsoleR;

public static partial class Console
{
    public static void Clear() {
        System.Console.Clear();
    }

    public static void ClearAll()
    {
        System.Console.SetCursorPosition(0, 0);
        Clear();
    }

    public static void WriteBool(bool value, string? trueMessage = null, string? falseMessage = null)
    {
        if (value)
            Success(trueMessage ?? "True");
        else
            Error(falseMessage ?? "False");
    }

    public static void WriteLine()
    {
        System.Console.WriteLine();
    }
    
    public static void WriteLine(string message, ConsoleColor? color = null)
    {
        DoWriteLine(message, color);
    }

    public static void WriteLine(string message, Color? color = null)
    {
        DoWriteLine(message, color);
    }

    public static void WriteLine(string message)
    {
        WriteLineFormatted(message);
    }

    public static void WriteLine(string message, string color)
    {
        DoWriteLine(message, color: color);
    }
    public static void Error(string message, bool showIcon = false)
    {
        if (showIcon) {
            message = (ConsoleHelpers.IsLegacy? "X " : "❌ ") + message;
        }
        DoWriteLine(message, ConsoleColor.Red);
    }

    public static void Success(string message, bool showIcon = false)
    {
        if (showIcon) {
            message = (ConsoleHelpers.IsLegacy ? "√ " : "✅ ") + message;
        }
        DoWriteLine(message, ConsoleColor.Green);
    }

    public static void Info(string message, bool showIcon = false)
    {
        if (showIcon) {
            message = (ConsoleHelpers.IsLegacy ? "i " :"ℹ️ ") + message;
        }
        DoWriteLine(message, ConsoleColor.Blue);
    }

    public static void Warning(string message, bool showIcon = false)
    {
        if (showIcon) {
            message = (ConsoleHelpers.IsLegacy ? "! " : "⚠️ ") + message;
        }
        DoWriteLine(message, ConsoleColor.Yellow);
    }

    private static void DoWriteLine(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
            System.Console.ForegroundColor = color.Value;

        WriteLineFormatted(message);
        System.Console.ResetColor();
    }

    
    private static void DoWriteLine(string message, Color? color = null)
    {
        if (color.HasValue)
            message = ConsoleHelpers.GetColorfulText(message, color.Value);

        WriteLineFormatted(message);
    }

    private static void DoWriteLine(string message, string? color = null)
    {
        if (!string.IsNullOrEmpty(color))
            message = ConsoleHelpers.GetColorfulText(message, color);

        WriteLineFormatted(message);
    }

    static void WriteLineFormatted(string input)
    {
        var regex = new Regex(@"\[(?<colors>[^\]]+)\](?<text>.*?)\[/\]",
            RegexOptions.Singleline);

        int lastIndex = 0;

        foreach (Match match in regex.Matches(input))
        {
            System.Console.Write(input[lastIndex..match.Index]);

            var colorParts = match.Groups["colors"].Value.Split(':');

            if (Enum.TryParse(colorParts[0], true, out ConsoleColor fg))
                ForegroundColor = fg;

            if (colorParts.Length > 1 &&
                Enum.TryParse(colorParts[1], true, out ConsoleColor bg))
                BackgroundColor = bg;

            System.Console.Write(match.Groups["text"].Value);

            ResetColor();

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < input.Length)
        {
            Write(input[lastIndex..]);
        }

        Write(Environment.NewLine);
    }

}
