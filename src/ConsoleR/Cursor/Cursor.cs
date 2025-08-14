namespace ConsoleR.Cursor;

public static partial class Console
{
    public static void HideCursor() => System.Console.CursorVisible = false;
    public static void ShowCursor() => System.Console.CursorVisible = true;
    public static void MoveCursorToStart() => System.Console.SetCursorPosition(0, System.Console.CursorTop);
    public static void MoveCursorBack(int steps) => System.Console.SetCursorPosition(System.Console.CursorLeft - steps, System.Console.CursorTop);
    public static void MoveCursorForward(int steps) => System.Console.SetCursorPosition(System.Console.CursorLeft + steps, System.Console.CursorTop);

    public static class Cursor
    {
        public static void Hide() => HideCursor();
        public static void Show() => ShowCursor();
        public static void MoveStart() => MoveCursorToStart();
        public static void MoveBack(int steps = 1) => MoveCursorBack(steps);
        public static void MoveNext(int steps = 1) => MoveCursorForward(steps);
    }
}
