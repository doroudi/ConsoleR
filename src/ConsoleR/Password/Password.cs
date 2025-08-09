using System.Security;

namespace ConsoleR;

public  static partial class Console {
    public static string Password(string prompt)
    {
        if (prompt is not null)
            Write(prompt);

        var password = "";
        do
        {
            var keyInfo = System.Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password[0..^1];
                    System.Console.Write("\b \b");
                }
            }
            else if (keyInfo.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
            {
                password+= keyInfo.KeyChar;
                Write("*");
            }
        } while (true);
        System.Console.WriteLine();
        return password;
    }
} 
