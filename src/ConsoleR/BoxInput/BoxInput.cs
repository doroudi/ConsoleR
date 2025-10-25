using System.Text;

namespace ConsoleR;

public partial class Console
{
    public static string ReadInBox(string title = "", ConsoleColor borderColor = ConsoleColor.White)
    {
        System.Console.OutputEncoding = Encoding.UTF8;
        var boxInput = BoxInput.Create(title, borderColor);
        return boxInput.ReadInput();
    }
}

public class BoxInput
{
    private static int _boxWidth;
    private static int _maxInputLength;
    private static string _title = "";
    private static ConsoleColor _borderColor;

    public static BoxInput Create(string title = "", ConsoleColor borderColor = ConsoleColor.White)
    {
        return new BoxInput(title, borderColor);
    }

    public BoxInput(string title = "", ConsoleColor borderColor = ConsoleColor.White)
    {
        _boxWidth = Console.WindowWidth - 4;
        _maxInputLength = _boxWidth - 4;
        _title = title;
        _borderColor = borderColor;
    }

    public string ReadInput()
    {
        int originalLeft = Console.CursorLeft;
        int originalTop = Console.CursorTop;

        try
        {
            DrawBox(originalLeft, originalTop);
            return GetInputInsideBox(originalLeft + 2, originalTop + 1);
        }
        finally
        {
            Console.Cursor.SetPosition(0, originalTop + 3);
        }
    }

    private void DrawBox(int left, int top)
    {
        Console.SetColor(_borderColor);
        Console.Cursor.SetPosition(left, top);
        
        if (ConsoleHelpers.IsLegacy)
            Console.WriteLine($"┌{"─"}{_title}{'─'.Repeat(_boxWidth - _title.Length - 3)}┐");
        else
            Console.WriteLine($"╭─{_title}{'─'.Repeat(_boxWidth - _title.Length - 3)}╮");

        Console.Cursor.SetPosition(left, top + 1);
        if (!string.IsNullOrEmpty(_title))
        {
            string paddedTitle = $" {_title} ";
            Console.Write(paddedTitle.PadRight(_boxWidth - 2));
        }

        Console.Cursor.SetPosition(left, top + 1);
        Console.SetColor(_borderColor);
        Console.Write("│" + new string(' ', _boxWidth - 2) + "│");

        Console.Cursor.SetPosition(left, top + 2);
        Console.SetColor(_borderColor);
        Console.Write("╰" + new string('─', _boxWidth - 2) + "╯");

        Console.ResetColor();
    }

    private string GetInputInsideBox(int inputLeft, int inputTop)
    {
        var input = new StringBuilder();
        int cursorPosition = 0;
        int displayStart = 0;

        Console.Cursor.SetPosition(inputLeft, inputTop);

        while (true)
        {
            var keyInfo = Console.ReadKey(true);
            var key = keyInfo.Key;

            if (key == ConsoleKey.Enter)
            {
                break;
            }
            else if (key == ConsoleKey.Backspace)
            {
                if (cursorPosition > 0)
                {
                    input.Remove(cursorPosition - 1, 1);
                    cursorPosition--;
                    RedisplayInput(input, inputLeft, inputTop, ref displayStart, cursorPosition);
                }
            }
            else if (key == ConsoleKey.Delete)
            {
                if (cursorPosition < input.Length)
                {
                    input.Remove(cursorPosition, 1);
                    RedisplayInput(input, inputLeft, inputTop, ref displayStart, cursorPosition);
                }
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                if (cursorPosition > 0)
                {
                    cursorPosition--;
                    UpdateCursorPosition(inputLeft, inputTop, displayStart, cursorPosition);
                }
            }
            else if (key == ConsoleKey.RightArrow)
            {
                if (cursorPosition < input.Length)
                {
                    cursorPosition++;
                    UpdateCursorPosition(inputLeft, inputTop, displayStart, cursorPosition);
                }
            }
            else if (key == ConsoleKey.Home)
            {
                cursorPosition = 0;
                UpdateCursorPosition(inputLeft, inputTop, displayStart, cursorPosition);
            }
            else if (key == ConsoleKey.End)
            {
                cursorPosition = input.Length;
                UpdateCursorPosition(inputLeft, inputTop, displayStart, cursorPosition);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                if (input.Length < _maxInputLength)
                {
                    input.Insert(cursorPosition, keyInfo.KeyChar);
                    cursorPosition++;
                    RedisplayInput(input, inputLeft, inputTop, ref displayStart, cursorPosition);
                }
            }
        }

        return input.ToString();
    }

    private void RedisplayInput(StringBuilder input, int inputLeft, int inputTop, ref int displayStart, int cursorPosition)
    {
        int displayWidth = _boxWidth - 4;

        if (cursorPosition < displayStart)
        {
            displayStart = cursorPosition;
        }
        else if (cursorPosition >= displayStart + displayWidth)
        {
            displayStart = cursorPosition - displayWidth + 1;
        }

        string displayText = input.ToString();
        if (displayText.Length > displayWidth)
        {
            int start = Math.Max(0, displayStart);
            int length = Math.Min(displayWidth, displayText.Length - start);
            displayText = displayText.Substring(start, length);
        }
        else
        {
            displayStart = 0;
        }

        Console.Cursor.SetPosition(inputLeft, inputTop);
        Console.Write(displayText.PadRight(displayWidth));

        UpdateCursorPosition(inputLeft, inputTop, displayStart, cursorPosition);
    }

    private void UpdateCursorPosition(int inputLeft, int inputTop, int displayStart, int cursorPosition)
    {
        int displayPosition = cursorPosition - displayStart;
        Console.Cursor.SetPosition(inputLeft + displayPosition, inputTop);
    }
}
