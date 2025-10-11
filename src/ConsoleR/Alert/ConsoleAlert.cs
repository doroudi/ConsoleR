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
    public static void Create(string message, string? title = null, MessageType type = MessageType.Info)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        var splitted = message.Split(Environment.NewLine);
        var totalMaxLength = splitted.Select(x => x.Length).Max();
        var stringLength = totalMaxLength + 4; // 4 for borders and spaces around
        var maxLength = Math.Min(stringLength, System.Console.WindowWidth); // Set a maximum length for each line
        var wrappedMessage = WrapText(message, maxLength - 4); // 4 for borders and spaces around
        

        title ??= "";
        Console.WriteLine(BuildHeader(title, maxLength), (ConsoleColor)type);
        foreach (var line in wrappedMessage)
        {
            Console.Write("│", (ConsoleColor)type);
            Console.Write($" {line.PadRight(maxLength - 4)} ");
            Console.Write("│\n", (ConsoleColor)type);
        }
        Console.WriteLine(BuildFooter(maxLength), (ConsoleColor)type);
    }

    private static string BuildHeader(string title, int maxLength)
    {
        int count = (int) Math.Ceiling((decimal)((maxLength - 2 - title.Length)/2));
        var isBalanced = count * 2 + title.Length >= (maxLength - 2) ;

        if(ConsoleHelpers.IsLegacy)
            return $"┌{'─'.Repeat(count)}{title}{'─'.Repeat(!isBalanced ? count + 1: count)}┐";
        else
            return $"╭{'─'.Repeat(count)}{title}{'─'.Repeat(!isBalanced ? count + 1 : count)}╮";
    }   
    
    private static string BuildFooter(int maxLength)
    {
        if (ConsoleHelpers.IsLegacy)
            return $"└{'─'.Repeat(maxLength - 2)}┘";
        else
            return $"╰{'─'.Repeat(maxLength - 2)}╯";
    }

    private static List<string> WrapText(string text, int maxLength)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = string.Empty;

        foreach (var word in words)
        {
            if ((currentLine + word).Length > maxLength || word == Environment.NewLine)
            {
                lines.Add(currentLine);
                currentLine = word.Replace(Environment.NewLine, "");
            }
            else
            {
                currentLine += (currentLine.Length > 0 ? " " : "") + word;
            }
        }

        if (currentLine.Length > 0)
        {
            lines.Add(currentLine);
        }

        return lines;
    }

}