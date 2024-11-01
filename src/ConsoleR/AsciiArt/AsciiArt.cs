using ConsoleR.AsciiArt.AsciiCharacters;

namespace ConsoleR.AsciiArt;

public static class AsciiArt
{
    public static string ToAsciiArt(string message) {
        return AsciiChars.GetAsciiArt(message);
    }
}
