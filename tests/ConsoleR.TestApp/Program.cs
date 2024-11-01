using ConsoleR;
using ConsoleR.AsciiArt;
const string message = "Hello, World!";

WriteLine.Print(message);
WriteLine.Error(message);
WriteLine.Warning(message);
WriteLine.Success(message);
WriteLine.Info(message);
WriteLine.PrintAsciiArt(message, MessageType.Warning);
Write.Success(AsciiArt.ToAsciiArt("ConsoleR"));
Write.Print(AsciiArt.ToAsciiArt("ConsoleR"));
Write.Error(AsciiArt.ToAsciiArt("ConsoleR"));