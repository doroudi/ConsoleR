namespace ConsoleR;

public static partial class Console {
    public static void Alert(string message, string title, ConsoleMessageType messageType) {
        ConsoleAlert.Create(message, title, messageType);
    }
}

internal static class ConsoleAlert {
    public static void Create(string message, string title, ConsoleMessageType type = ConsoleMessageType.Info) 
    {
        var maxLength = message.Length + 4; // 4 for borders and spaces around

        title ??= "";
        
        Console.WriteLine(BuildHeader(title,type, maxLength), (ConsoleColor)type);
        Console.Write("|", (ConsoleColor)type);
        Console.Write($" {message} ");
        Console.Write("|\n", (ConsoleColor)type);
        Console.WriteLine(BuildFooter(type, maxLength), (ConsoleColor)type);
    }

    private static string BuildHeader(string title, ConsoleMessageType type, int maxLength)
    {

        int count = (int) Math.Ceiling((decimal)((maxLength - 2 - title.Length)/2));
        var isBalanced = count * 2 + title.Length >= (maxLength - 2) ;

        var str = $"┌{'─'.Repeat(count)}{title}{'─'.Repeat(!isBalanced ? count + 1: count)}┐";

        return str;
    }
    
    private static string BuildFooter(ConsoleMessageType type, int maxLength)
    {
        return $"└{'─'.Repeat(maxLength - 2)}┘";
    }
}

public enum ConsoleMessageType: int {
    None = 0,
    Info = ConsoleColor.Blue,
    Success = ConsoleColor.Green,
    Warning = ConsoleColor.DarkYellow,
    Error = ConsoleColor.Red,
}