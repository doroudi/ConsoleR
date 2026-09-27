using ConsoleR.AsciiArt.AsciiCharacters;

namespace ConsoleR;

public static partial class Console
{
    /// <summary>
    /// Writes a message as ASCII art with the strokes of the outline font.
    /// </summary>
    /// <param name="message">Text to draw, letters that the font does not know are left blank.</param>
    /// <param name="color">Color of the art, the default color of the console when it is <c>null</c>.</param>
    public static void AsciiArt(string message, ConsoleColor? color = null)
    {
        WriteLine(AsciiChars.GetAsciiArt(message, AsciiArtStyle.Outline), color);
    }

    /// <summary>
    /// Writes a message as ASCII art, either with its outline, with solid strokes or with a shadow
    /// that falls to the bottom right of the letters.
    /// </summary>
    /// <param name="message">Text to draw, letters that the font does not know are left blank.</param>
    /// <param name="style">Look of the art.</param>
    /// <param name="color">Color of the letters, the default color of the console when it is <c>null</c>.</param>
    /// <param name="shadowColor">Color of the shadow, <see cref="ConsoleColor.DarkGray"/> when it is <c>null</c>.</param>
    public static void AsciiArt(string message, AsciiArtStyle style, ConsoleColor? color = null, ConsoleColor? shadowColor = null)
    {
        if (style != AsciiArtStyle.Shadow)
        {
            WriteLine(AsciiChars.GetAsciiArt(message, style), color);
            return;
        }

        var shadow = shadowColor ?? ConsoleColor.DarkGray;

        foreach (var row in AsciiChars.BuildRows(message, style))
        {
            foreach (var part in row.Parts)
            {
                ConsoleHelpers.ApplyColors(part.Shadow ? shadow : color);
                Write(part.Text);
            }

            WriteLine();
        }

        ResetColor();
        WriteLine();
    }
}
