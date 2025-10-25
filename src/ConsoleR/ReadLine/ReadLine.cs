namespace ConsoleR;

public partial class Console
{
    public static ConsoleKeyInfo ReadKey()
    {
        return System.Console.ReadKey();
    }

    public static ConsoleKeyInfo ReadKey(bool intercept)
    {
        return System.Console.ReadKey(intercept);
    }

    public static ConsoleKeyInfo ReadKey(string message)
    {
        Write(message);
        return System.Console.ReadKey();
    }

    public static string? ReadLine(string? prompt, string? defaultValue = null, ConsoleColor? color = null)
    {
        System.Console.Write(prompt);
        if (string.IsNullOrEmpty(defaultValue))
            return System.Console.ReadLine();
        return ReadWithEditableValue(defaultValue, color);
    }

    public static string? Read(string? prompt, string? defaultValue = null, ConsoleColor? color = null)
    {
        System.Console.Write(prompt);
        if(string.IsNullOrEmpty(defaultValue))
            return System.Console.ReadLine();
    
        return ReadWithEditableValue(defaultValue, color);
    }


    #region PrivateMethods
    private static string ReadWithEditableValue(string defaultValue, ConsoleColor? color = null)
    {
        int pos = System.Console.CursorLeft;
        Write(defaultValue, ConsoleColor.DarkGray);
        ConsoleKeyInfo info;
        List<char> chars = [];
        if (!string.IsNullOrEmpty(defaultValue))
        {
            chars.AddRange(defaultValue.ToCharArray());
        }

        if(color is not null)
            SetColor(color.Value);
        while (true)
        {
            info = System.Console.ReadKey(true);
            if (info.Key == ConsoleKey.Backspace && System.Console.CursorLeft > pos)
            {
                chars.RemoveAt(chars.Count - 1);
                System.Console.CursorLeft -= 1;
                System.Console.Write(' ');
                System.Console.CursorLeft -= 1;

            }
            else if (info.Key == ConsoleKey.Enter) { Write(Environment.NewLine); break; }
            else 
            {
                System.Console.Write(info.KeyChar);
                chars.Add(info.KeyChar);
            }
        }
        if(color is not null)
            ResetColor();
        return new string([.. chars]);
    }
    #endregion
}
