namespace ConsoleR;

public static partial class Console {
    public static void Alert(string message, string title, MessageType messageType = MessageType.None)
    {
        ConsoleAlert.Create(message, title, messageType);
    }
    public static void Alert(string message, MessageType messageType = MessageType.None)
    {
        ConsoleAlert.Create(message, "", messageType);
    }
}

internal static class ConsoleAlert {
    /// <summary>Columns the box spends on its borders and the spaces around the message.</summary>
    private const int BoxPadding = 4;

    public static void Create(string message, string? title = null, MessageType type = MessageType.Info)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;

        var (windowWidth, _) = ConsoleHelpers.GetWindowSize();
        var color = (ConsoleColor)type;

        // The message is wrapped first: the longest line of it decides how wide the box becomes and no
        // line can be wider than the window.
        var lines = WrapText(message, Math.Max(1, windowWidth - BoxPadding));

        var textWidth = 0;
        foreach (var line in lines) textWidth = Math.Max(textWidth, line.Length);

        var maxLength = Math.Min(textWidth + BoxPadding, windowWidth);
        var innerWidth = Math.Max(1, maxLength - BoxPadding);

        Console.WriteLine(BuildHeader(title ?? string.Empty, maxLength), color);
        foreach (var line in lines)
        {
            Console.Write("│", color);
            Console.Write($" {line.PadRight(innerWidth)} ");
            Console.Write("│\n", color);
        }
        Console.WriteLine(BuildFooter(maxLength), color);
    }

    /// <summary>
    /// Lines of the message, a line break of any kind starts a new line of the box and a line that is
    /// wider than the box is wrapped.
    /// </summary>
    private static List<string> WrapText(string? message, int maxLength)
    {
        var lines = new List<string>();
        var paragraphs = (message ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        foreach (var paragraph in paragraphs)
            lines.AddRange(ConsoleHelpers.WrapText(paragraph, maxLength));

        return lines;
    }

    /// <summary>Top border of the box with the title in it, exactly as wide as the box.</summary>
    private static string BuildHeader(string title, int maxLength)
    {
        var left = ConsoleHelpers.IsLegacy ? "┌" : "╭";
        var right = ConsoleHelpers.IsLegacy ? "┐" : "╮";
        var fill = Math.Max(0, maxLength - 2);

        var label = (title ?? string.Empty).Trim();
        if (label.Length == 0) return left + '─'.Repeat(fill) + right;

        // The title keeps a space on both sides and has to leave room for the corners.
        if (label.Length > fill - 2) label = label.Substring(0, Math.Max(0, fill - 2));
        label = $" {label} ";

        var remaining = Math.Max(0, fill - label.Length);
        var before = remaining / 2;

        return left + '─'.Repeat(before) + label + '─'.Repeat(remaining - before) + right;
    }

    /// <summary>Bottom border of the box, exactly as wide as the box.</summary>
    private static string BuildFooter(int maxLength)
    {
        var inner = Math.Max(0, maxLength - 2);
        return ConsoleHelpers.IsLegacy
            ? "└" + '─'.Repeat(inner) + "┘"
            : "╰" + '─'.Repeat(inner) + "╯";
    }
}
