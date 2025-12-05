using System.Drawing;
using static System.Console;

namespace ConsoleR;

internal static class ConsoleHelpers {
    private const string Sequence_Code_Foreground = "\u001b[38;2;{0};{1};{2}m";
    private const string Sequence_Code_Background = "\u001b[48;2;{0};{1};{2}m";
    private const string Sequence_Code_Foreground_Default = "\u001b[39m";
    private const string Sequence_Code_Background_Default = "\u001b[49m";


    /// <summary>
    /// Check whether the console is running in a legacy mode. (legacy console is not supporting UTF-8 or UTF-16 encoding)
    /// </summary>
    public static bool IsLegacy =>
        Environment.OSVersion.Platform == PlatformID.Win32NT &&
            (OutputEncoding.CodePage != 1200 /* UTF-16 */ && OutputEncoding.CodePage != 65001 /* UTF-8 */);

    /// <summary>
    /// Check whether app is running in Windows Terminal.
    /// </summary>
    public static bool IsWindowsTerminal =>
        Environment.GetEnvironmentVariable("WT_SESSION") != null;

    /// <summary>
    /// Set the console foreground color using RGB values.
    /// </summary>
    /// <param name="color">System.Drawing.Color</param>
    private static string GetColorSentence(Color color) {
        return string.Format(Sequence_Code_Foreground, color.R.ToString(), color.G.ToString(), color.B.ToString());
    }


    /// <summary>
    /// Set the console foreground color using a hex color string.
    /// </summary>
    /// <param name="colorHex">Hex color code for example: #22ED12</param>
    public static string GetColorfulOutput(string colorHex)
    {
        var color = Color.FromArgb(int.Parse(colorHex.StartsWith("#") ? colorHex.Substring(1) : colorHex, System.Globalization.NumberStyles.HexNumber));
        return string.Format(Sequence_Code_Foreground, color.R.ToString(), color.G.ToString(), color.B.ToString());
    }

    /// <summary>
    /// Write Set the console foreground color using a hex color string.
    /// </summary>
    /// <param name="text">Text to print with specific color</param>
    /// <param name="colorHex">Hex color code for example: #22ED12</param>
    public static string GetColorfulText(string text, string colorHex)
    {
        var color = Color.FromArgb(int.Parse(colorHex.StartsWith("#") ? colorHex.Substring(1) : colorHex, System.Globalization.NumberStyles.HexNumber));
        return $"{GetColorSentence(color)}{text}{Sequence_Code_Foreground_Default}";
    }

    /// <summary>
    /// Write Set the console foreground color using a hex color string.
    /// </summary>
    /// <param name="text">Text to print with specific color</param>
    /// <param name="color">Color of text</param>
    public static string GetColorfulText(string text, Color color)
    {
        var colorFormatted = string.Format(Sequence_Code_Foreground, color.R.ToString(), color.G.ToString(), color.B.ToString());
        return $"{colorFormatted}{text}{Sequence_Code_Foreground_Default}";
    }
}