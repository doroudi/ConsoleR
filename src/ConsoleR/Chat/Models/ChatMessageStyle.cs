namespace ConsoleR.Chat.Models;

/// <summary>
/// Look of the messages of one <see cref="ConsoleChatRole"/>: the label in front of every message and the
/// colors of the label and of the message text. A color that is <c>null</c> uses the default color
/// of the console.
/// </summary>
public class ChatMessageStyle
{
    /// <summary>
    /// Text printed in front of the first line of the message, followed by <c>": "</c>.
    /// An empty label prints the message without a name, and the wrapped lines are not indented.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Color of the message text.</summary>
    public ConsoleColor? Foreground { get; set; }

    /// <summary>
    /// Background color of the message text. The whole width of the line is filled with it,
    /// so every message is a solid block.
    /// </summary>
    public ConsoleColor? Background { get; set; }

    /// <summary>Color of the label, <see cref="Foreground"/> when it is <c>null</c>.</summary>
    public ConsoleColor? LabelForeground { get; set; }

    /// <summary>Background color of the label, <see cref="Background"/> when it is <c>null</c>.</summary>
    public ConsoleColor? LabelBackground { get; set; }
}
