using System.Text;

namespace ConsoleR.AsciiArt.AsciiCharacters;

/// <summary>One piece of a row of the art, drawn in the color of the letters or of their shadow.</summary>
internal readonly struct AsciiArtPart
{
    public AsciiArtPart(string text, bool shadow)
    {
        Text = text ?? string.Empty;
        Shadow = shadow;
    }

    public string Text { get; }

    /// <summary>The piece belongs to the shadow of the letters.</summary>
    public bool Shadow { get; }
}

/// <summary>One row of the art, built from pieces that are drawn in different colors.</summary>
internal sealed class AsciiArtRow
{
    public AsciiArtRow(IReadOnlyList<AsciiArtPart> parts)
    {
        Parts = parts ?? Array.Empty<AsciiArtPart>();
    }

    public IReadOnlyList<AsciiArtPart> Parts { get; }

    public override string ToString()
    {
        var text = new StringBuilder();
        foreach (var part in Parts) text.Append(part.Text);
        return text.ToString();
    }
}

internal static class AsciiChars
{
  /// <summary>Rows of the letters of the font.</summary>
  private const int GlyphRows = 6;

  /// <summary>Width a character without a letter of its own takes, for example a space.</summary>
  private const int UnknownCharWidth = 7;

  static readonly Dictionary<char, string[]> charMaps = new()
    {
        { 'A', ["    _    ", @"   / \   ",@"  / _ \  ",@" / ___ \ ", @"/_/   \_\"] },
        { 'B', [" ____  ","| __ ) ",@"|  _ \ ","| |_) |","|____/ "]},
        { 'C', ["  ____ "," / ___|","| |    ","| |___ ",@" \____|"]},
        { 'D', [" ____  ",@"|  _ \ ",@"| | | |",@"| |_| |",@"|____/ "]},
        { 'E', [" _____ ","| ____|","|  _|  ","| |___ ","|_____|",]},
        { 'F', [" _____ ","|  ___|","| |_   ","|  _|  ","|_|    ",]},
        { 'G', ["  ____ "," / ___|","| |  _ ","| |_| |",@" \____|"  ]},
        { 'H', [" _   _ ","| | | |","| |_| |","|  _  |","|_| |_|"]},
        { 'I', [" ___ ","|_ _|"," | | "," | | ","|___|"]},
        { 'J', ["     _ ","    | |"," _  | |","| |_| |",@" \___/ "]},
        { 'K', [" _  __","| |/ /","| ' / ",@"| . \ ",@"|_|\_\"]},
        { 'L', [" _     ","| |    ","| |    ","| |___ ","|_____|"]},
        { 'M', [" __  __ ",@"|  \/  |",@"| |\/| |","| |  | |","|_|  |_|"] },
        { 'N', [" _   _ ", @"| \ | |", @"|  \| |", @"| |\  |", @"|_| \_|"] },
        { 'O', ["  ___  ",@" / _ \ ","| | | |","| |_| |",@" \___/ "] },
        { 'P', [" ____  ",@"|  _ \ ","| |_) |","|  __/ ","|_|    "] },
        { 'Q', ["  ___  ",@" / _ \ ","| | | |","| |_| |",@" \__\_\"] },
        { 'R', [" ____  ",@"|  _ \ ","| |_) |","|  _ < ",@"|_| \_\"] },
        { 'S', [" ____  ","/ ___| ",@"\___ \ "," ___) |","|____/ "] },
        { 'T', [" _____ ","|_   _|","  | |  ","  | |  ","  |_|  "] },
        { 'U', [" _   _ ","| | | |","| | | |","| |_| |",@" \___/ "] },
        { 'V', ["__     __",@"\ \   / /",@" \ \ / / ",@"  \ V /  ",@"   \_/   "] },
        { 'W', ["__        __",@"\ \      / /",@" \ \ /\ / / ",@"  \ V  V /  ",@"   \_/\_/   "] },
        { 'X', ["__  __",@"\ \/ /",@" \  / ",@" /  \ ",@"/_/\_\"] },
        { 'Y', ["__   __",@"\ \ / /",@" \ V / ","  | |  ","  |_|  "] },
        { 'Z', [" _____","|__  /","  / / "," / /_ ","/____|"] },
        { 'a', ["       ","  __ _ "," / _` |","| (_| |",@" \__,_|"] },
        { 'b', [" _     ","| |__  ",@"| '_ \ ","| |_) |","|_.__/ "] },
        { 'c', ["      ","  ___ "," / __|","| (__ ",@" \___|"] },
        { 'd', ["     _ ","  __| |"," / _` |","| (_| |",@" \__,_|"] },
        { 'e', ["      ","  ___ ",@" / _ \","|  __/",@" \___|"] },
        { 'f', ["  __ "," / _|","| |_ ","|  _|","|_|  "] },
        { 'g', ["       ","  __ _ "," / _` |","| (_| |",@" \__, |"," |___/ "] },
        { 'h', [" _     ","| |__  ",@"| '_ \ ","| | | |","|_| |_|"] },
        { 'i', [" _ ", "(_)", "| |", "| |", "|_|"] },
        { 'j', ["   _ ","  (_)","  | |"," _/ |","|__/ "] },
        { 'k', [" _    ","| | __","| |/ /","|   < ",@"|_|\_\"] },
        { 'l', [" _ ","| |","| |","| |","|_|"] },
        { 'm', ["           "," _ __ ___  ",@"| '_ ` _ \ ","| | | | | |","|_| |_| |_|"] },
        { 'n', ["       "," _ __  ",@"| '_ \ ","| | | |","|_| |_|"] },
        { 'o', ["       ", "  ___  ", @" / _ \ ", "| (_) |", @" \___/ "] },
        { 'p', ["       "," _ __  ",@"| '_ \ ","| |_) |","| .__/ ","|_|    "] },
        { 'q', ["       ","  __ _ ", " / _` |", "| (_| |", @" \__, |", "    |_|"] },
        { 'r', ["      ", " _ __ ", "| '__|", "| |   ", "|_|   "] },
        { 's', ["     "," ___ ","/ __|",@"\__ \","|___/"] },
        { 't', [" _   ", "| |_ ", "| __|", "| |_ ", @" \__|"] },
        { 'u', ["       "," _   _ ","| | | |","| |_| |",@" \__,_|"] },
        { 'v', ["       ","__   __",@"\ \ / /",@" \ V / ",@"  \_/  "] },
        { 'w', ["          ","__      __",@"\ \ /\ / /",@" \ V  V / ",@"  \_/\_/  "] },
        { 'x', ["      ","__  __",@"\ \/ /"," >  < ",@"/_/\_\"] },
        { 'y', ["       "," _   _ ","| | | |","| |_| |",@" \__, |"," |___/ "] },
        { 'z', ["   "," ____","|_  /"," / / ","/___|"] }
    };

  /// <summary>Character a stroke of a letter is drawn with in the filled style.</summary>
  private static char FillSign => ConsoleHelpers.IsLegacy ? '#' : '█';

  /// <summary>Character the shadow of the letters is drawn with.</summary>
  private static char ShadowSign => ConsoleHelpers.IsLegacy ? '#' : '░';

  /// <summary>
  /// The whole art as plain text, every row ends with a line break.
  /// </summary>
  public static string GetAsciiArt(string text, AsciiArtStyle style)
  {
    var sb = new StringBuilder();
    foreach (var row in BuildRows(text, style)) sb.AppendLine(row.ToString());
    return sb.ToString();
  }

  /// <summary>
  /// Rows of the art of the given style. Every row knows which of its pieces are the shadow of the
  /// letters, so both can be drawn in their own color.
  /// </summary>
  public static List<AsciiArtRow> BuildRows(string text, AsciiArtStyle style)
  {
    var value = text ?? string.Empty;
    var glyphs = new List<string>(GlyphRows);
    var letters = new List<(int Start, int Width)>(value.Length);

    var position = 0;
    foreach (var character in value)
    {
      var letterWidth = GetGlyphWidth(character);
      letters.Add((position, letterWidth));
      position += letterWidth;
    }

    for (var row = 0; row < GlyphRows; row++)
    {
      var line = new StringBuilder(position);
      foreach (var character in value) line.Append(GetGlyphRow(character, row));
      glyphs.Add(line.ToString());
    }

    if (style == AsciiArtStyle.Filled)
    {
      for (var row = 0; row < glyphs.Count; row++) glyphs[row] = Fill(glyphs[row]);
    }

    return style == AsciiArtStyle.Shadow ? AddShadow(glyphs, letters) : ToRows(glyphs, null);
  }

  /// <summary>One row of a single character, padded to the width of the whole character.</summary>
  private static string GetGlyphRow(char character, int row)
  {
    if (!charMaps.TryGetValue(character, out var glyph)) return new string(' ', UnknownCharWidth);

    var width = GetGlyphWidth(glyph);
    var text = row < glyph.Length ? glyph[row] : string.Empty;
    return text.Length < width ? text.PadRight(width) : text;
  }

  /// <summary>Width of a character, the longest row of its letter.</summary>
  private static int GetGlyphWidth(char character) =>
    charMaps.TryGetValue(character, out var glyph) ? GetGlyphWidth(glyph) : UnknownCharWidth;

  /// <summary>Width of a character, the longest row of its letter.</summary>
  private static int GetGlyphWidth(string[] glyph)
  {
    var width = 0;
    foreach (var row in glyph) width = Math.Max(width, row.Length);
    return width;
  }

  /// <summary>Turns the strokes of the letters into solid blocks.</summary>
  private static string Fill(string line)
  {
    var filled = new StringBuilder(line.Length);
    foreach (var character in line) filled.Append(char.IsWhiteSpace(character) ? character : FillSign);
    return filled.ToString();
  }

  /// <summary>
  /// Gives every letter the shadow of its own body: the body of a letter is drawn again one row lower
  /// and one column further right, so the shadow falls out of the right and the bottom of a letter and
  /// the holes inside a letter keep their distance from it.
  /// </summary>
  private static List<AsciiArtRow> AddShadow(IReadOnlyList<string> glyphs, IReadOnlyList<(int Start, int Width)> letters)
  {
    var width = 1;
    foreach (var line in glyphs) width = Math.Max(width, line.Length + 1);

    var canvas = new char[glyphs.Count + 1][];
    var shadow = new bool[glyphs.Count + 1][];
    for (var row = 0; row < canvas.Length; row++)
    {
      canvas[row] = new string(' ', width).ToCharArray();
      shadow[row] = new bool[width];
    }

    for (var row = 0; row < glyphs.Count; row++)
      for (var column = 0; column < glyphs[row].Length; column++) canvas[row][column] = glyphs[row][column];

    foreach (var letter in letters)
    {
      var from = letter.Start;
      var to = letter.Start + letter.Width - 1;

      for (var row = 0; row < glyphs.Count; row++)
      {
        var body = GetBody(glyphs[row], from, to);
        if (body.First < 0) continue;

        var next = row + 1 < glyphs.Count ? GetBody(glyphs[row + 1], from, to) : (First: -1, Last: -1);

        for (var column = body.First; column <= body.Last; column++)
        {
          var target = column + 1;
          if (target >= width) continue;

          // The letter stays on top of its shadow and the shadow never reaches into the letter.
          if (!char.IsWhiteSpace(canvas[row + 1][target])) continue;
          if (target >= next.First && target <= next.Last) continue;

          canvas[row + 1][target] = ShadowSign;
          shadow[row + 1][target] = true;
        }
      }
    }

    var lines = new string[canvas.Length];
    for (var row = 0; row < canvas.Length; row++) lines[row] = new string(canvas[row]);

    // Letters without a tail leave the last rows of the canvas empty, they would only add blank lines.
    var height = lines.Length;
    while (height > 1 && lines[height - 1].Trim().Length == 0) height--;

    var trimmed = new string[height];
    var trimmedShadow = new bool[height][];
    for (var row = 0; row < height; row++)
    {
      trimmed[row] = lines[row];
      trimmedShadow[row] = shadow[row];
    }

    return ToRows(trimmed, trimmedShadow);
  }

  /// <summary>First and last column a letter uses in one of its rows, both <c>-1</c> in an empty row.</summary>
  private static (int First, int Last) GetBody(string line, int from, int to)
  {
    var first = -1;
    var last = -1;

    for (var column = Math.Max(0, from); column <= Math.Min(to, line.Length - 1); column++)
    {
      if (char.IsWhiteSpace(line[column])) continue;

      if (first < 0) first = column;
      last = column;
    }

    return (first, last);
  }

  /// <summary>
  /// Turns the characters of the canvas into the pieces of a row, neighbours with the same look
  /// share a piece.
  /// </summary>
  private static List<AsciiArtRow> ToRows(IReadOnlyList<string> lines, IReadOnlyList<bool[]>? shadow)
  {
    var rows = new List<AsciiArtRow>(lines.Count);

    for (var row = 0; row < lines.Count; row++)
    {
      var parts = new List<AsciiArtPart>(2);
      var text = lines[row];
      var current = new StringBuilder();
      var currentIsShadow = false;

      for (var column = 0; column < text.Length; column++)
      {
        var isShadow = shadow != null && shadow[row][column];

        if (current.Length > 0 && isShadow != currentIsShadow)
        {
          parts.Add(new AsciiArtPart(current.ToString(), currentIsShadow));
          current.Clear();
        }

        currentIsShadow = isShadow;
        current.Append(text[column]);
      }

      if (current.Length > 0) parts.Add(new AsciiArtPart(current.ToString(), currentIsShadow));
      rows.Add(new AsciiArtRow(parts));
    }

    return rows;
  }
}
