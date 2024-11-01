namespace ConsoleR;

public static class WriteLine
{
    public static void PrintAsciiArt(string message, MessageType messageType = MessageType.None) {
        Print(AsciiArt.AsciiArt.ToAsciiArt(message), messageType);
    }
    public static void Print(string message, MessageType messageType = MessageType.None)
    {
        switch (messageType)
        {
            case MessageType.Success:
                Write(message, ConsoleColor.Green);
                break;
            case MessageType.Info:
                Write(message, ConsoleColor.Blue);
                break;
            case MessageType.Warning:
                Write(message, ConsoleColor.Yellow);
                break;
            case MessageType.Error:
                Write(message, ConsoleColor.Red);
                break;
            default:
                Write(message);
                break;
        }
    }

    public static void Error(string message)
    {
        Write(message, ConsoleColor.Red);
    }

    public static void Success(string message)
    {
        Write(message, ConsoleColor.Green);
    }

    public static void Info(string message)
    {
        Write(message, ConsoleColor.Blue);
    }

    public static void Warning(string message)
    {
        Write(message, ConsoleColor.Yellow);
    }


    private static void Write(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
        {
            Console.ForegroundColor = color.Value;
        }

        Console.WriteLine(message);
        Console.ResetColor();
    }
}
