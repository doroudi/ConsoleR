using System.Drawing;
using static System.Console;

namespace ConsoleR;

internal static class ConsoleHelpers {
    private const string Sequence_Code_Foreground = "\u001b[38;2;{0};{1};{2}m";
    private const string Sequence_Code_Background = "\u001b[48;2;{0};{1};{2}m";
    private const string Sequence_Code_Foreground_Default = "\u001b[39m";
    private const string Sequence_Code_Background_Default = "\u001b[49m";

    /// <summary>Size assumed when the console window cannot be measured.</summary>
    public const int FallbackWindowWidth = 80;

    /// <summary>Height assumed when the console window cannot be measured.</summary>
    public const int FallbackWindowHeight = 25;


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
    /// Size of the console window, with a sane fallback for redirected output.
    /// </summary>
    public static (int Width, int Height) GetWindowSize() {
        try {
            var width = System.Console.WindowWidth;
            var height = System.Console.WindowHeight;
            if (width > 0 && height > 0) return (width, height);
        }
        catch (IOException) { }
        catch (PlatformNotSupportedException) { }
        catch (ArgumentOutOfRangeException) { }

        return (FallbackWindowWidth, FallbackWindowHeight);
    }

    /// <summary>
    /// Positioning the cursor requires a real, not redirected, output.
    /// </summary>
    public static bool CanPositionCursor() {
        try {
            return !System.Console.IsOutputRedirected;
        }
        catch (IOException) {
            return false;
        }
    }

    /// <summary>
    /// Breaks a text into lines that are not longer than the given width.
    /// </summary>
    public static List<string> WrapText(string text, int width) {
        var lines = new List<string>();
        if (width < 1) width = 1;

        var current = new System.Text.StringBuilder();

        foreach (var word in text.Split(' ')) {
            var candidate = current.Length == 0 ? word : current + " " + word;

            if (candidate.Length <= width) {
                current.Clear();
                current.Append(candidate);
                continue;
            }

            if (current.Length > 0) {
                lines.Add(current.ToString());
                current.Clear();
            }

            // A single word can be longer than a whole line, break it apart.
            var remaining = word;
            while (remaining.Length > width) {
                lines.Add(remaining.Substring(0, width));
                remaining = remaining.Substring(width);
            }

            current.Append(remaining);
        }

        if (current.Length > 0 || lines.Count == 0) lines.Add(current.ToString());

        return lines;
    }

    /// <summary>
    /// Sets the colors of the console. A color that is <c>null</c> falls back to the default color of the console.
    /// </summary>
    public static void ApplyColors(ConsoleColor? foreground, ConsoleColor? background = null) {
        System.Console.ResetColor();

        if (foreground.HasValue) System.Console.ForegroundColor = foreground.Value;
        if (background.HasValue) System.Console.BackgroundColor = background.Value;
    }

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