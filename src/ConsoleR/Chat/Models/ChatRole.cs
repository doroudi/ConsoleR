namespace ConsoleR.Chat.Models;

/// <summary>
/// Who wrote a message of the chat history.
/// </summary>
public enum ChatRole
{
    /// <summary>A message the user typed.</summary>
    User,

    /// <summary>A message the bot answered.</summary>
    Bot
}
