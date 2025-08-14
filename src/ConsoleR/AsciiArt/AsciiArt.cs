using ConsoleR.AsciiArt.AsciiCharacters;

namespace ConsoleR;

public static partial class Console
{
    public static void AsciiArt(string message, ConsoleColor? color = null)
    {
        WriteLine(AsciiChars.GetAsciiArt2(message), color);
    }
}
