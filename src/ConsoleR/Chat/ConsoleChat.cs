using System.Runtime.ExceptionServices;
using System.Text;
using ConsoleR.Chat.Models;
using ConsoleR.Loading;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ConsoleR;

public static partial class Console
{
    /// <summary>
    /// Creates an AI chat in the console: the history fills the window from the top and the input box
    /// sticks to the bottom. <paramref name="reply"/> is called with every message the user sends and
    /// returns the answer of the bot, so any model or service can be plugged in.
    /// </summary>
    /// <param name="reply">
    /// Answer of the bot for a message of the user. Returning <c>null</c> adds no message,
    /// which is what a reply that streams itself into the chat with
    /// <see cref="ConsoleChat.AddMessage"/> and <see cref="ConsoleChat.UpdateLastMessage"/> returns.
    /// </param>
    /// <param name="settings">Look of the chat, the styles of the user and the bot are set here.</param>
    public static ConsoleChat Chat(Func<string, string?> reply, ConsoleChatSettings? settings = null)
    {
        return ConsoleChat.Create(reply, settings);
    }

    /// <summary>
    /// Creates an AI chat in the console: the history fills the window from the top and the input box
    /// sticks to the bottom. <paramref name="replyAsync"/> is awaited with every message the user sends
    /// and returns the answer of the bot, so any model or service can be plugged in.
    /// </summary>
    /// <param name="replyAsync">
    /// Answer of the bot for a message of the user. Returning <c>null</c> adds no message,
    /// which is what a reply that streams itself into the chat with
    /// <see cref="ConsoleChat.AddMessage"/> and <see cref="ConsoleChat.UpdateLastMessage"/> returns.
    /// </param>
    /// <param name="settings">Look of the chat, the styles of the user and the bot are set here.</param>
    public static ConsoleChat Chat(Func<string, Task<string?>> replyAsync, ConsoleChatSettings? settings = null)
    {
        return ConsoleChat.Create(replyAsync, settings);
    }

    /// <summary>
    /// Creates an AI chat in the console that streams its reply. The delegate returns an async stream of
    /// partial message contents; the chat will add the first chunk as a new bot message and update its
    /// content for subsequent chunks.
    /// </summary>
    public static ConsoleChat Chat(Func<string, IAsyncEnumerable<string?>> replyStreamAsync, ConsoleChatSettings? settings = null)
    {
        return ConsoleChat.Create(replyStreamAsync, settings);
    }
}

/// <summary>
/// A chat in the console with the history on top and the input box sticky at the bottom of the window.
/// The chat only draws, the answers come from the reply that is handed to <see cref="ConsoleR.Console.Chat(Func{string,string?},ConsoleChatSettings?)"/>.
/// </summary>
public sealed class ConsoleChat
{
    private readonly List<ConsoleChatMessage> _messages = new();
    private readonly ConsoleChatSettings _settings;
    private readonly Func<string, Task<string?>> _reply;
    private readonly Func<string, IAsyncEnumerable<string?>>? _replyStream;
    private readonly StringBuilder _input = new();

    /// <summary>Position of the caret inside the input.</summary>
    private int _cursor;

    /// <summary>History lines that are scrolled out of sight, <see cref="int.MaxValue"/> keeps the newest line visible.</summary>
    private int _scroll = int.MaxValue;

    /// <summary>First row of the input that is visible in the box.</summary>
    private int _inputScroll;

    private int _maxScroll;
    private int _caretRow;
    private int _caretColumn;

    private ChatLayout _layout;
    private bool _hasRendered;
    private bool _thinking;

    /// <summary>Creates a chat that answers through the given delegate.</summary>
    public static ConsoleChat Create(Func<string, string?> reply, ConsoleChatSettings? settings = null)
    {
        return new ConsoleChat(settings, message => Task.FromResult(reply(message)));
    }

    /// <summary>Creates a chat that answers through the given asynchronous delegate.</summary>
    public static ConsoleChat Create(Func<string, Task<string?>> replyAsync, ConsoleChatSettings? settings = null)
    {
        return new ConsoleChat(settings, replyAsync);
    }

    /// <summary>Creates a chat that streams its reply via an async enumerable of partial strings.</summary>
    public static ConsoleChat Create(Func<string, IAsyncEnumerable<string?>> replyAsync, ConsoleChatSettings? settings = null)
    {
        return new ConsoleChat(settings, reply: null, replyStream: replyAsync);
    }

    private ConsoleChat(ConsoleChatSettings? settings, Func<string, Task<string?>>? reply = null, Func<string, IAsyncEnumerable<string?>>? replyStream = null)
    {
        System.Console.OutputEncoding = Encoding.UTF8;

        _settings = settings ?? new ConsoleChatSettings();
        _reply = reply ?? (message => Task.FromResult<string?>(null));
        _replyStream = replyStream;
    }

    /// <summary>Look of the chat, changes take effect with the next render.</summary>
    public ConsoleChatSettings Settings => _settings;

    /// <summary>Messages of the chat in the order they were added.</summary>
    public IReadOnlyList<ConsoleChatMessage> Messages => _messages;

    /// <summary>
    /// Adds a message to the history and draws it. Before <see cref="Show"/>, <see cref="Run"/> or
    /// <see cref="ReadInput"/> the message is only remembered.
    /// </summary>
    public void AddMessage(ConsoleChatRole role, string? text = null)
    {
        _messages.Add(new ConsoleChatMessage(role, text));
        _scroll = int.MaxValue;
        if (_hasRendered) Render();
    }

    /// <summary>
    /// Replaces the text of the last message, so an answer that arrives in pieces can grow in the chat.
    /// Does nothing when the history is empty.
    /// </summary>
    public void UpdateLastMessage(string? text)
    {
        if (_messages.Count == 0) return;

        _messages[_messages.Count - 1].Text = text ?? string.Empty;
        if (_hasRendered) Render();
    }

    /// <summary>Draws the whole chat on a cleared screen.</summary>
    public void Show()
    {
        Render(fullRedraw: true);
    }

    /// <summary>
    /// Runs the chat until the user leaves it with <c>Escape</c>. The answers of the reply are awaited.
    /// </summary>
    public async Task RunAsync()
    {
        Show();

        while (true)
        {
            var text = ReadInput();
            if (text == null) return;

            text = text.Trim();
            if (text.Length == 0) continue;

            AddMessage(ConsoleChatRole.User, text);
            await ReplyAsync(text);
        }
    }

    /// <summary>
    /// Runs the chat until the user leaves it with <c>Escape</c>. Blocks the calling thread while the
    /// answer of the bot is on the way.
    /// </summary>
    public void Run()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Reads a single message in the input box. <c>Enter</c> sends it, <c>Shift+Enter</c> and
    /// <c>Ctrl+Enter</c> start a new line, <c>Escape</c> ends the chat and returns <c>null</c>.
    /// The arrow keys scroll the history, <c>PageUp</c>, <c>PageDown</c> and <c>End</c> jump through it.
    /// </summary>
    /// <returns>Text that was typed, <c>null</c> when the chat was left.</returns>
    public string? ReadInput()
    {
        if (!_hasRendered) Show();

        var cursorVisible = TryGetCursorVisible();
        TrySetCursorVisible(true);

        try
        {
            while (true)
            {
                Render();
                MoveCursorToInput();

                var key = System.Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                    {
                        // A newline belongs in the message, only a bare Enter sends it.
                        if ((key.Modifiers & (ConsoleModifiers.Shift | ConsoleModifiers.Control)) != 0)
                        {
                            Insert('\n');
                            break;
                        }

                        var text = _input.ToString();
                        _input.Clear();
                        _cursor = 0;
                        _inputScroll = 0;
                        return text;
                    }

                    case ConsoleKey.Escape:
                        return null;

                    case ConsoleKey.Backspace:
                        if (_cursor > 0)
                        {
                            _input.Remove(_cursor - 1, 1);
                            _cursor--;
                        }
                        break;

                    case ConsoleKey.Delete:
                        if (_cursor < _input.Length) _input.Remove(_cursor, 1);
                        break;

                    case ConsoleKey.LeftArrow:
                        if (_cursor > 0) _cursor--;
                        break;

                    case ConsoleKey.RightArrow:
                        if (_cursor < _input.Length) _cursor++;
                        break;

                    case ConsoleKey.Home:
                        _cursor = LineStart(_cursor);
                        break;

                    case ConsoleKey.End:
                        _cursor = LineEnd(_cursor);
                        break;

                    case ConsoleKey.UpArrow:
                        Scroll(-1);
                        break;

                    case ConsoleKey.DownArrow:
                        Scroll(1);
                        break;

                    case ConsoleKey.PageUp:
                        Scroll(-Math.Max(1, _layout.HistoryRows));
                        break;

                    case ConsoleKey.PageDown:
                        Scroll(Math.Max(1, _layout.HistoryRows));
                        break;

                    default:
                        if (!char.IsControl(key.KeyChar)) Insert(key.KeyChar);
                        break;
                }
            }
        }
        finally
        {
            TrySetCursorVisible(cursorVisible);
            MoveCursorOutOfChat();
        }
    }

    /// <summary>Adds a character to the input at the caret, a line break starts a new line.</summary>
    private void Insert(char value)
    {
        _input.Insert(_cursor, value);
        _cursor++;
    }

    /// <summary>Start of the line the given position belongs to.</summary>
    private int LineStart(int index)
    {
        if (index <= 0) return 0;

        var lineBreak = _input.ToString().LastIndexOf('\n', index - 1);
        return lineBreak < 0 ? 0 : lineBreak + 1;
    }

    /// <summary>End of the line the given position belongs to.</summary>
    private int LineEnd(int index)
    {
        var lineBreak = _input.ToString().IndexOf('\n', index);
        return lineBreak < 0 ? _input.Length : lineBreak;
    }

    /// <summary>
    /// Asks the reply of the bot. The spinner of the library runs in the input box while the answer
    /// is on the way.
    /// </summary>
    private async Task ReplyAsync(string message)
    {
        _thinking = true;
        Render();

        // The caret belongs to the user, it is hidden while the bot is at work.
        var cursorVisible = TryGetCursorVisible();
        TrySetCursorVisible(false);

        string? answer = null;
        Exception? failure = null;

        try
        {
            if (UseSpinner)
            {
                var streamed = false;
                // The spinner draws on its own thread, it stops as soon as the answer is there.
                var spinner = new Spinner();
                await spinner.Start(
                    async () =>
                    {
                        try
                        {
                            if (_replyStream != null)
                            {
                                StringBuilder sb = null;
                                await foreach (var part in _replyStream(message))
                                {
                                    if (part == null) continue;
                                    if (sb == null)
                                    {
                                        sb = new StringBuilder(part);
                                        AddMessage(ConsoleChatRole.Bot, sb.ToString());
                                      }
                                    else
                                    {
                                        sb.Append(part);
                                        UpdateLastMessage(sb.ToString());
                                    }
                                }
                                // stream handled the updates itself
                                answer = null;
                            }
                            else
                            {
                                answer = await _reply(message);
                            }
                        }
                        catch (Exception exception)
                        {
                            // The spinner swallows the exception, the chat reports it after its screen is back.
                            failure = exception;
                        }
                    },
                    _settings.ThinkingText);
            }
            else
            {
                try
                {
                    if (_replyStream != null)
                    {
                        StringBuilder sb = null;
                        await foreach (var part in _replyStream(message))
                        {
                            if (part == null) continue;
                            if (sb == null)
                            {
                                sb = new StringBuilder(part);
                                AddMessage(ConsoleChatRole.Bot, sb.ToString());
                            }
                            else
                            {
                                sb.Append(part);
                                UpdateLastMessage(sb.ToString());
                            }
                        }
                        answer = null;
                    }
                    else
                    {
                        answer = await _reply(message);
                    }
                }
                catch (Exception exception)
                {
                    failure = exception;
                }
            }
        }
        finally
        {
            _thinking = false;
            TrySetCursorVisible(cursorVisible);
        }

        if (failure != null)
        {
            Render();
            ExceptionDispatchInfo.Capture(failure).Throw();
        }

        // A reply that is null already drew its own message, the chat only leaves the thinking state.
        if (answer == null) Render();
        else AddMessage(ConsoleChatRole.Bot, answer);
    }

    /// <summary>
    /// The spinner owns the row it animates on, so it is only used when the input box is drawn on a
    /// console that can be painted.
    /// </summary>
    private bool UseSpinner => _layout.ShowInputBox && ConsoleHelpers.CanPositionCursor();

    /// <summary>Scrolls the history by the given number of lines, the newest line is the bottom.</summary>
    private void Scroll(int lines)
    {
        var offset = Math.Min(_scroll, _maxScroll);
        var next = offset + lines;

        // Reaching the last line pins the chat to the newest one again.
        _scroll = next >= _maxScroll ? int.MaxValue : Math.Max(0, next);
    }

    /// <summary>Draws the chat. The screen is cleared when the window size changed.</summary>
    private void Render(bool fullRedraw = false)
    {
        var (windowWidth, windowHeight) = ConsoleHelpers.GetWindowSize();

        var screen = ChatRenderer.BuildScreen(
            _messages,
            _settings,
            windowWidth,
            windowHeight,
            _scroll,
            _inputScroll,
            _input.ToString(),
            _cursor,
            _thinking);

        var redraw = fullRedraw || !_hasRendered || !screen.Layout.Equals(_layout);
        ChatRenderer.Draw(screen.Rows, screen.Layout, redraw);

        _maxScroll = screen.MaxScroll;
        _inputScroll = screen.InputScroll;
        _caretRow = screen.CursorRow;
        _caretColumn = screen.CursorColumn;
        _layout = screen.Layout;
        _hasRendered = true;

        // The spinner of the bot draws on the row of the caret, it has to find the cursor there.
        if (_thinking && UseSpinner) TrySetCursorPosition(0, _layout.InputTextTop);
    }

    private void MoveCursorToInput()
    {
        if (!ConsoleHelpers.CanPositionCursor()) return;

        var column = Math.Min(Math.Max(0, _caretColumn), Math.Max(0, _layout.Width - 1));
        TrySetCursorPosition(column, _caretRow);
    }

    /// <summary>Leaves the cursor on the last row of the chat, so the next output starts below it.</summary>
    private void MoveCursorOutOfChat()
    {
        if (!ConsoleHelpers.CanPositionCursor()) return;

        var row = Math.Min(_layout.LastRow + 1, Math.Max(0, _layout.Height - 1));
        TrySetCursorPosition(0, row);
    }

    private static void TrySetCursorPosition(int left, int top)
    {
        try
        {
            System.Console.SetCursorPosition(left, top);
        }
        catch (IOException) { }
        catch (ArgumentOutOfRangeException) { }
    }

    private static bool TryGetCursorVisible()
    {
        try
        {
            return System.Console.CursorVisible;
        }
        catch (IOException) { return true; }
        catch (PlatformNotSupportedException) { return true; }
    }

    private static void TrySetCursorVisible(bool visible)
    {
        try
        {
            System.Console.CursorVisible = visible;
        }
        catch (IOException) { }
        catch (PlatformNotSupportedException) { }
    }
}
