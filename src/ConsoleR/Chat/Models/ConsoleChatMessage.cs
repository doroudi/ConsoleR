namespace ConsoleR.Chat.Models;

/// <summary>
/// A single message of the chat history (<see cref="ConsoleR.ConsoleChat"/>).
/// </summary>
public sealed class ConsoleChatMessage
{
    public ConsoleChatMessage(ConsoleChatRole role, string? text = null)
    {
        Role = role;
        Text = text ?? string.Empty;
    }

    /// <summary>Who wrote the message.</summary>
    public ConsoleChatRole Role { get; }

    /// <summary>
    /// Text of the message. It is wrapped to the width of the window when the chat is drawn,
    /// <c>\n</c> starts a new line.
    /// </summary>
    public string Text { get; internal set; }
}
