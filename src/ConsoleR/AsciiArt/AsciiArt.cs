using ConsoleR.AsciiArt.AsciiCharacters;

namespace ConsoleR;

public static partial class Consoler
{
    public static void AsciiArt(string message, ConsoleColor? color = null) {
        WriteLine(AsciiChars.GetAsciiArt(message), color);
    }
}
