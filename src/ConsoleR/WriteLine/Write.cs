namespace ConsoleR;

public static partial class Consoler
{

    public static void Write(string message, ConsoleColor? color = null)
    {
        DoWrite(message, color);
    }


    private static void DoWrite(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
            Console.ForegroundColor = color.Value;
        
        Console.Write(message);

        if (color.HasValue)
            Console.ResetColor();
    }
}
