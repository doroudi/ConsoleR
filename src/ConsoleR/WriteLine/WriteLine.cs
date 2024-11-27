namespace ConsoleR;

public static partial class Console
{
    public static ConsoleKeyInfo ReadKey() {
        return System.Console.ReadKey();
    }

    public static string? ReadLine() {
        return System.Console.ReadLine();
    }

    public static string? ReadLine(string? prompt) {
        System.Console.WriteLine(prompt);
        return System.Console.ReadLine();
    }

    public static string? Read(string? prompt)
    {
        System.Console.Write(prompt);
        return System.Console.ReadLine();
    }

    public static void WriteLine(string message, ConsoleColor? color = null) 
    {
        DoWriteLine(message,color);
    }


    public static void Error(string message, bool showIcon = true)
    {
        if (showIcon) {
            message = "❌ " + message;
        }
        DoWriteLine(message, ConsoleColor.Red);
    }

    public static void Success(string message, bool showIcon = true)
    {
        if (showIcon) {
            message = "✅ " + message;
        }
        DoWriteLine(message, ConsoleColor.Green);
    }

    public static void Info(string message, bool showIcon = true)
    {
        if (showIcon) {
            message = "ℹ️ " + message;
        }
        DoWriteLine(message, ConsoleColor.Blue);
    }

    public static void Warning(string message, bool showIcon = true)
    {
        if (showIcon) {
            message = "⚠️ " + message;
        }
        DoWriteLine(message, ConsoleColor.Yellow);
    }


    private static void DoWriteLine(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
        {
            System.Console.ForegroundColor = color.Value;
        }

        System.Console.WriteLine(message);
        System.Console.ResetColor();
    }
}
