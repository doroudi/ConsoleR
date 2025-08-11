using static System.Console;

namespace ConsoleR;

internal static class ConsoleHelpers {
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
}