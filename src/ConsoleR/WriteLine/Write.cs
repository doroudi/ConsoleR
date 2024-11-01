namespace ConsoleR;

public static class Write
{
    public static void Print(string message, MessageType messageType = MessageType.None)
    {
        switch (messageType)
        {
            case MessageType.Success:
                write(message, ConsoleColor.Green);
                break;
            case MessageType.Info:
                write(message, ConsoleColor.Blue);
                break;
            case MessageType.Warning:
                write(message, ConsoleColor.Yellow);
                break;
            case MessageType.Error:
                write(message, ConsoleColor.Red);
                break;
            default:
                write(message);
                break;
        }
    }

    public static void Error(string message)
    {
        write(message, ConsoleColor.Red);
    }

    public static void Success(string message)
    {
        write(message, ConsoleColor.Green);
    }

    public static void Info(string message)
    {
        write(message, ConsoleColor.Blue);
    }

    public static void Warning(string message)
    {
        write(message, ConsoleColor.Yellow);
    }


    private static void write(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
        {
            Console.ForegroundColor = color.Value;
        }

        Console.Write(message);
        Console.ResetColor();
    }
}
