namespace ConsoleR.Chat.Models;

/// <summary>
/// Configuration of the chat UI (<see cref="ConsoleR.ConsoleChat"/>). The colors of the user messages
/// and of the bot messages are set separately, so both sides of the conversation look different.
/// </summary>
public class ChatSettings
{
    /// <summary>
    /// Text shown in the border of the input box, for example <c>"You"</c>. An empty title shows a plain border.
    /// </summary>
    public string InputTitle { get; set; } = "You";

    /// <summary>Color of the borders of the input box.</summary>
    public ConsoleColor BorderColor { get; set; } = ConsoleColor.DarkGray;

    /// <summary>Color of the text that is being typed into the input box.</summary>
    public ConsoleColor? InputColor { get; set; } = ConsoleColor.White;

    /// <summary>
    /// How far the input box grows upwards while the message gets longer, the history keeps the rest
    /// of the window. When <c>null</c> (or less than one) the box grows as far as the window allows.
    /// </summary>
    public int? MaxInputLines { get; set; } = 5;

    /// <summary>
    /// Text shown in the input box while the answer of the bot is awaited. The spinner of the library
    /// runs in front of it.
    /// </summary>
    public string ThinkingText { get; set; } = "Thinking...";

    /// <summary>Look of the messages the user typed.</summary>
    public ChatMessageStyle UserStyle { get; set; } = new ChatMessageStyle
    {
        Label = "You",
        Foreground = ConsoleColor.Cyan
    };

    /// <summary>Look of the messages of the bot.</summary>
    public ChatMessageStyle BotStyle { get; set; } = new ChatMessageStyle
    {
        Label = "Bot",
        Foreground = ConsoleColor.Green
    };

    /// <summary>Style of the messages of the given role.</summary>
    internal ChatMessageStyle StyleOf(ChatRole role) => role == ChatRole.User ? UserStyle : BotStyle;
}
