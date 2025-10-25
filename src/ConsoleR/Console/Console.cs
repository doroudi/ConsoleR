namespace ConsoleR;

public partial class Console
{
    public static int WindowWidth => System.Console.WindowWidth;
    public static int WindowHeight => System.Console.WindowHeight;
    public static int WindowLeft => System.Console.WindowLeft;
    public static int WindowTop => System.Console.WindowTop;
    public static int CursorLeft => System.Console.CursorLeft;
    public static int CursorTop => System.Console.CursorTop;
    public static int CursorSize => System.Console.CursorSize;
    public static bool CursorVisible => System.Console.CursorVisible;

    public static void SetColor(ConsoleColor color) => System.Console.ForegroundColor = color;
    public static void SetBackgroundColor(ConsoleColor color) => System.Console.BackgroundColor = color;
    public static ConsoleColor ForegroundColor {get => System.Console.ForegroundColor; set => System.Console.ForegroundColor = value; }
    public static ConsoleColor BackgroundColor { get => System.Console.BackgroundColor; set => System.Console.BackgroundColor = value; }
    public static void ResetColor() => System.Console.ResetColor();
}
