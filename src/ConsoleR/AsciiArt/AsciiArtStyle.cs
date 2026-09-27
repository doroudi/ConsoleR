namespace ConsoleR;

/// <summary>
/// Look of the output of <see cref="ConsoleR.Console.AsciiArt(string, AsciiArtStyle, ConsoleColor?, ConsoleColor?)"/>.
/// </summary>
public enum AsciiArtStyle
{
    /// <summary>The letters are drawn with their strokes, the holes of a letter stay open.</summary>
    Outline = 0,

    /// <summary>The strokes of the letters are drawn as solid blocks.</summary>
    Filled = 1,

    /// <summary>The letters keep their strokes and throw a shadow to the bottom right.</summary>
    Shadow = 2
}
