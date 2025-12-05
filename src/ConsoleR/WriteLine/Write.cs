using System.Drawing;

namespace ConsoleR;

public static partial class Console
{
    public static void Write(string message)
    {
        System.Console.Write(message);
    }

    public static void Write(string message, ConsoleColor? color = null)
    {
        DoWrite(message, color);
    }

    public static void Write(string message, Color? color = null)
    {
        DoWrite(message, color);
    }

    public static void Write(string message, string? color = null)
    {
        DoWrite(message, color);
    }

    private static void DoWrite(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
            System.Console.ForegroundColor = color.Value;
        
        System.Console.Write(message);

        if (color.HasValue)
            System.Console.ResetColor();
    }

    private static void DoWrite(string message, Color? color = null)
    {
        if (color.HasValue)
            message = ConsoleHelpers.GetColorfulText(message, color.Value);

        System.Console.Write(message);
    }

    private static void DoWrite(string message, string? color = null)
    {
        if (!string.IsNullOrEmpty(color))
            message = ConsoleHelpers.GetColorfulText(message, color);

        System.Console.Write(message);
    }
}
