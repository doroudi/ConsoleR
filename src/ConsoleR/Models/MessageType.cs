namespace ConsoleR;

public enum MessageType : int
{
    None = 0,
    Info = ConsoleColor.Blue,
    Success = ConsoleColor.Green,
    Warning = ConsoleColor.DarkYellow,
    Error = ConsoleColor.Red,
}