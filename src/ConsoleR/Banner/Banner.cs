namespace ConsoleR;

public static partial class Console {
    public static void Banner(string text, ConsoleColor? color = null) {
        const int total = 64;
        var spaces = (total - text.Length)/2;


        var formattedText = $"{new string(' ', spaces)}{text}{new string(' ', spaces)}";
        var message = 
        @$"
        +---------------------------------------------------------------+
        |                                                               |
        |                                                               |
        |                                                               |
        |                                                               |
        |{formattedText}|
        |                                                               |
        |                                                               |
        |                                                               |
        |                                                               |
        +---------------------------------------------------------------+";

        WriteLine(message,color);
    }
}