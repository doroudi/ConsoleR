using ConsoleR.Chat.Models;

namespace ConsoleR;

/// <summary>One colored piece of a line of the chat screen.</summary>
internal readonly struct ChatSpan
{
    public ChatSpan(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        Text = text ?? string.Empty;
        Foreground = foreground;
        Background = background;
    }

    public string Text { get; }

    /// <summary>Color of the text, <c>null</c> means the default color of the console.</summary>
    public ConsoleColor? Foreground { get; }

    public ConsoleColor? Background { get; }
}

/// <summary>
/// A line of the chat screen, built from one or more colored pieces. A message line is made of the
/// label and the text of the message, both can be drawn in their own colors.
/// </summary>
internal sealed class ChatLine
{
    public static readonly ChatLine Empty = new(Array.Empty<ChatSpan>());

    public ChatLine(IReadOnlyList<ChatSpan> spans)
    {
        Spans = spans ?? Array.Empty<ChatSpan>();
    }

    public IReadOnlyList<ChatSpan> Spans { get; }

    /// <summary>Number of characters of the line.</summary>
    public int Length
    {
        get
        {
            var length = 0;
            for (var i = 0; i < Spans.Count; i++) length += Spans[i].Text.Length;
            return length;
        }
    }

    /// <summary>Colors of the piece the line ends with, the rest of the row is filled with them.</summary>
    public ChatSpan Tail => Spans.Count > 0 ? Spans[Spans.Count - 1] : default;
}

/// <summary>One line of the chat screen together with the console row it is drawn on.</summary>
internal readonly struct ChatRow
{
    public ChatRow(int row, ChatLine line)
    {
        Row = row;
        Line = line;
    }

    public int Row { get; }

    public ChatLine Line { get; }
}

/// <summary>
/// Geometry of a single render of the chat: the history fills the window from the top, the input box
/// sticks to the bottom and grows upwards with the text that is typed into it.
/// </summary>
internal readonly struct ChatLayout : IEquatable<ChatLayout>
{
    public ChatLayout(
        int width,
        int height,
        int historyRows,
        bool showInputBox,
        int inputTopRow,
        int inputTextTop,
        int inputTextRows,
        int inputBottomRow,
        int inputWidth)
    {
        Width = width;
        Height = height;
        HistoryRows = historyRows;
        ShowInputBox = showInputBox;
        InputTopRow = inputTopRow;
        InputTextTop = inputTextTop;
        InputTextRows = inputTextRows;
        InputBottomRow = inputBottomRow;
        InputWidth = inputWidth;
    }

    /// <summary>Width every row is padded to. It is one column shorter than the window, so drawing never scrolls.</summary>
    public int Width { get; }

    public int Height { get; }

    /// <summary>Rows of the window that show the chat history.</summary>
    public int HistoryRows { get; }

    /// <summary>The input box is drawn with a border. In a very short window it is a single row instead.</summary>
    public bool ShowInputBox { get; }

    /// <summary>Row of the top border of the input box, <c>-1</c> without a box.</summary>
    public int InputTopRow { get; }

    /// <summary>First row the user types on.</summary>
    public int InputTextTop { get; }

    /// <summary>Rows the input is drawn on, the box grows with the text up to the configured limit.</summary>
    public int InputTextRows { get; }

    /// <summary>Row of the bottom border of the input box, <c>-1</c> without a box.</summary>
    public int InputBottomRow { get; }

    /// <summary>Number of characters of the input that fit into a row of the box.</summary>
    public int InputWidth { get; }

    /// <summary>Column the input starts at, the box adds a border and a space in front of it.</summary>
    public int InputLeft => ShowInputBox ? 2 : 0;

    /// <summary>Last row the chat uses.</summary>
    public int LastRow => ShowInputBox ? InputBottomRow : InputTextTop + InputTextRows - 1;

    public bool Equals(ChatLayout other) =>
        Width == other.Width &&
        Height == other.Height &&
        HistoryRows == other.HistoryRows &&
        ShowInputBox == other.ShowInputBox &&
        InputTopRow == other.InputTopRow &&
        InputTextTop == other.InputTextTop &&
        InputTextRows == other.InputTextRows &&
        InputBottomRow == other.InputBottomRow &&
        InputWidth == other.InputWidth;

    public override bool Equals(object? obj) => obj is ChatLayout other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(Width, Height, HistoryRows, ShowInputBox, InputTextTop, InputTextRows, InputWidth);
}

/// <summary>
/// The input text broken into the rows of the input box, together with the position of the caret.
/// A line of the input that is wider than the box continues on the row below it.
/// </summary>
internal readonly struct InputView
{
    public InputView(IReadOnlyList<string> lines, int caretLine, int caretColumn)
    {
        Lines = lines;
        CaretLine = caretLine;
        CaretColumn = caretColumn;
    }

    /// <summary>Rows the input needs, every line is at most as wide as the box.</summary>
    public IReadOnlyList<string> Lines { get; }

    /// <summary>Row of the caret inside <see cref="Lines"/>.</summary>
    public int CaretLine { get; }

    /// <summary>Column of the caret inside its row.</summary>
    public int CaretColumn { get; }
}

/// <summary>Every row of one render of the chat and the state the chat needs to place the cursor.</summary>
internal sealed class ChatScreen
{
    public ChatScreen(
        ChatLayout layout,
        IReadOnlyList<ChatRow> rows,
        int maxScroll,
        int inputScroll,
        int cursorRow,
        int cursorColumn)
    {
        Layout = layout;
        Rows = rows;
        MaxScroll = maxScroll;
        InputScroll = inputScroll;
        CursorRow = cursorRow;
        CursorColumn = cursorColumn;
    }

    public ChatLayout Layout { get; }

    public IReadOnlyList<ChatRow> Rows { get; }

    /// <summary>Lines of the history that are hidden above the visible part.</summary>
    public int MaxScroll { get; }

    /// <summary>Rows of the input that are hidden above the visible part of the box.</summary>
    public int InputScroll { get; }

    /// <summary>Row the caret is drawn on.</summary>
    public int CursorRow { get; }

    /// <summary>Column the caret is drawn in.</summary>
    public int CursorColumn { get; }
}

/// <summary>
/// Turns the chat history and the input into the lines of the chat screen and draws them on the console.
/// </summary>
internal static class ChatRenderer
{
    /// <summary>Rows of the input box that are not text: top and bottom border.</summary>
    private const int InputBoxBorderRows = 2;

    /// <summary>Rows of a borderless input.</summary>
    private const int InputRows = 1;

    /// <summary>A window has to be at least this high to show the input inside a box.</summary>
    private const int MinHeightForBox = 4;

    /// <summary>A window has to be at least this wide to show the input inside a box.</summary>
    private const int MinWidthForBox = 8;

    /// <summary>Rows the history and the input box need together, at least one row stays for the history.</summary>
    private const int RowsKeptForHistory = 3;

    private static bool IsLegacy => ConsoleHelpers.IsLegacy;

    /// <summary>
    /// Places the history on top and the input box at the bottom of the window. The history takes
    /// every row that is left above the input, so the input never moves out of sight.
    /// <paramref name="inputLines"/> is the number of rows the input currently needs,
    /// <paramref name="maxInputLines"/> limits how far the box grows (<c>null</c> or less than one
    /// grows it as far as the window allows).
    /// </summary>
    public static ChatLayout ComputeLayout(int windowWidth, int windowHeight, int inputLines, int? maxInputLines)
    {
        // One column of the window stays free, so that no row ever scrolls the console.
        var width = Math.Max(1, windowWidth - 1);
        var height = Math.Max(1, windowHeight);

        var showInputBox = height >= MinHeightForBox && width >= MinWidthForBox;
        var textRows = InputRows;

        if (showInputBox)
        {
            var limit = maxInputLines.GetValueOrDefault();
            if (limit < 1) limit = int.MaxValue;
            limit = Math.Min(limit, Math.Max(1, height - RowsKeptForHistory));
            textRows = Math.Min(Math.Max(1, inputLines), limit);
        }

        var inputRows = textRows + (showInputBox ? InputBoxBorderRows : 0);
        var historyRows = Math.Max(0, height - inputRows);

        var inputTopRow = historyRows;
        var inputTextTop = showInputBox ? inputTopRow + 1 : inputTopRow;
        var inputBottomRow = showInputBox ? inputTextTop + textRows : -1;

        // The box keeps a border and a space on both sides of the text.
        var inputWidth = Math.Max(1, showInputBox ? width - 4 : width);

        return new ChatLayout(
            width,
            height,
            historyRows,
            showInputBox,
            inputTopRow,
            inputTextTop,
            textRows,
            inputBottomRow,
            inputWidth);
    }

    /// <summary>
    /// Breaks the input into the rows of the box: a line break starts a new row and a line that is
    /// wider than the box continues on the next row. The caret is mapped to its row and column.
    /// </summary>
    public static InputView WrapInput(string? text, int caret, int width)
    {
        var value = text ?? string.Empty;
        if (width < 1) width = 1;
        if (caret < 0) caret = 0;
        if (caret > value.Length) caret = value.Length;

        var lines = new List<string>();
        var caretLine = 0;
        var caretColumn = 0;

        var start = 0;
        while (true)
        {
            var end = value.IndexOf('\n', start);
            var hasLineBreak = end >= 0;
            if (!hasLineBreak) end = value.Length;

            var length = end - start;
            var rowCount = Math.Max(1, (int)Math.Ceiling(length / (double)width));

            if (caret >= start && caret <= end)
            {
                var offset = caret - start;
                var row = offset / width;
                var column = offset % width;

                // A caret behind the last character of a full row hangs at the end of that row.
                if (row >= rowCount)
                {
                    row = rowCount - 1;
                    column = width;
                }

                caretLine = lines.Count + row;
                caretColumn = column;
            }

            for (var row = 0; row < rowCount; row++)
            {
                var from = start + row * width;
                lines.Add(value.Substring(from, Math.Min(width, end - from)));
            }

            if (!hasLineBreak) break;
            start = end + 1;
        }

        return new InputView(lines, caretLine, caretColumn);
    }

    /// <summary>
    /// First row of the input that is visible in a box of the given height. The box only scrolls as
    /// far as it has to, so the caret stays in sight.
    /// </summary>
    public static int GetInputScroll(int caretLine, int lineCount, int visibleRows, int previous)
    {
        if (visibleRows < 1) visibleRows = 1;

        var scroll = previous;
        if (caretLine < scroll) scroll = caretLine;
        else if (caretLine >= scroll + visibleRows) scroll = caretLine - visibleRows + 1;

        var maxScroll = Math.Max(0, lineCount - visibleRows);
        return Math.Min(Math.Max(0, scroll), maxScroll);
    }

    /// <summary>
    /// Builds the lines of a single message: the label is printed in front of the first line and the
    /// wrapped lines are indented to the width of the label.
    /// </summary>
    public static List<ChatLine> BuildMessageLines(ConsoleChatMessage message, ChatMessageStyle style, int width)
    {
        var lines = new List<ChatLine>();
        if (width < 1) width = 1;

        style ??= new ChatMessageStyle();

        var bodyForeground = style.Foreground;
        var bodyBackground = style.Background;
        var labelForeground = style.LabelForeground ?? bodyForeground;
        var labelBackground = style.LabelBackground ?? bodyBackground;

        // A label that leaves no room for the message itself is dropped.
        var label = string.IsNullOrEmpty(style.Label) ? string.Empty : style.Label + ": ";
        if (label.Length > width - 1) label = string.Empty;
        var indent = label.Length;

        var wrapWidth = Math.Max(1, width - indent);
        var paragraphs = (message.Text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        var first = true;
        foreach (var paragraph in paragraphs)
        {
            foreach (var wrapped in ConsoleHelpers.WrapText(paragraph, wrapWidth))
            {
                var spans = new List<ChatSpan>(2);

                if (first && indent > 0) spans.Add(new ChatSpan(label, labelForeground, labelBackground));
                else if (indent > 0) spans.Add(new ChatSpan(' '.Repeat(indent), bodyForeground, bodyBackground));

                spans.Add(new ChatSpan(wrapped, bodyForeground, bodyBackground));

                lines.Add(new ChatLine(spans));
                first = false;
            }
        }

        return lines;
    }

    /// <summary>
    /// Every line of the history, messages follow each other in the order they were added.
    /// </summary>
    public static List<ChatLine> BuildHistoryLines(IReadOnlyList<ConsoleChatMessage> messages, ConsoleChatSettings settings, int width)
    {
        var lines = new List<ChatLine>();
        if (messages == null) return lines;

        for (var i = 0; i < messages.Count; i++)
            lines.AddRange(BuildMessageLines(messages[i], settings.StyleOf(messages[i].Role), width));

        return lines;
    }

    /// <summary>
    /// Builds every row of the chat screen: the visible part of the history and the input box.
    /// </summary>
    /// <param name="messages">Messages of the chat in the order they were added.</param>
    /// <param name="settings">Look of the chat.</param>
    /// <param name="windowWidth">Width of the console window.</param>
    /// <param name="windowHeight">Height of the console window.</param>
    /// <param name="scroll">Lines of the history that are scrolled out of sight, <see cref="int.MaxValue"/> keeps the newest line visible.</param>
    /// <param name="inputScroll">First row of the input that is visible in the box.</param>
    /// <param name="input">Text the user has typed so far.</param>
    /// <param name="cursor">Position of the caret inside <paramref name="input"/>.</param>
    /// <param name="thinking">The input shows the text that stands in for the answer of the bot.</param>
    public static ChatScreen BuildScreen(
        IReadOnlyList<ConsoleChatMessage> messages,
        ConsoleChatSettings settings,
        int windowWidth,
        int windowHeight,
        int scroll,
        int inputScroll,
        string input,
        int cursor,
        bool thinking)
    {
        // The width of the box only depends on the window, so it is known before the text is wrapped.
        var layout = ComputeLayout(windowWidth, windowHeight, 1, settings.MaxInputLines);

        var text = thinking ? settings.ThinkingText ?? string.Empty : input;

        // The caret is hidden while the bot answers, so the box shows the text from its first row.
        var caret = thinking ? 0 : cursor;
        var view = WrapInput(text, caret, layout.InputWidth);

        // The box grows with the input, so the layout is built once more with the number of rows it needs.
        if (view.Lines.Count > 1) layout = ComputeLayout(windowWidth, windowHeight, view.Lines.Count, settings.MaxInputLines);

        var lines = BuildHistoryLines(messages, settings, layout.Width);
        var maxScroll = Math.Max(0, lines.Count - layout.HistoryRows);
        var offset = Math.Min(Math.Max(0, scroll), maxScroll);
        var rows = new List<ChatRow>(layout.HistoryRows + layout.InputTextRows + InputBoxBorderRows);

        for (var i = 0; i < layout.HistoryRows; i++)
        {
            var index = offset + i;
            rows.Add(new ChatRow(i, index < lines.Count ? lines[index] : ChatLine.Empty));
        }

        var boxScroll = GetInputScroll(view.CaretLine, view.Lines.Count, layout.InputTextRows, inputScroll);
        AddInputRows(rows, settings, layout, view, boxScroll, thinking);

        var cursorRow = layout.InputTextTop + Math.Min(Math.Max(0, view.CaretLine - boxScroll), layout.InputTextRows - 1);
        var cursorColumn = layout.InputLeft + Math.Min(Math.Max(0, view.CaretColumn), layout.InputWidth);

        return new ChatScreen(layout, rows, maxScroll, boxScroll, cursorRow, cursorColumn);
    }

    /// <summary>Rows of the input box, the typed text or the text shown while the bot is answering.</summary>
    private static void AddInputRows(
        List<ChatRow> rows,
        ConsoleChatSettings settings,
        ChatLayout layout,
        InputView view,
        int scroll,
        bool thinking)
    {
        var border = settings.BorderColor;
        var foreground = thinking ? ConsoleColor.DarkGray : settings.InputColor;

        if (layout.ShowInputBox)
        {
            rows.Add(new ChatRow(layout.InputTopRow, new ChatLine(new[]
            {
                new ChatSpan(TopBorder(settings.InputTitle, layout.Width), border)
            })));
        }

        for (var row = 0; row < layout.InputTextRows; row++)
        {
            var index = scroll + row;
            var text = index >= 0 && index < view.Lines.Count ? view.Lines[index] : string.Empty;
            if (text.Length > layout.InputWidth) text = text.Substring(0, layout.InputWidth);

            var visible = text.PadRight(layout.InputWidth);

            rows.Add(new ChatRow(
                layout.InputTextTop + row,
                layout.ShowInputBox
                    ? new ChatLine(new[]
                    {
                        new ChatSpan(IsLegacy ? "| " : "│ ", border),
                        new ChatSpan(visible, foreground),
                        new ChatSpan(IsLegacy ? " |" : " │", border)
                    })
                    : new ChatLine(new[]
                    {
                        new ChatSpan(visible, foreground)
                    })));
        }

        if (layout.ShowInputBox)
        {
            rows.Add(new ChatRow(layout.InputBottomRow, new ChatLine(new[]
            {
                new ChatSpan(BottomBorder(layout.Width), border)
            })));
        }
    }

    /// <summary>Top border of the input box with the title in it, for example <c>╭─ You ────╮</c>.</summary>
    private static string TopBorder(string? title, int width)
    {
        var text = title ?? string.Empty;
        if (text.Length > width - 5) text = text.Substring(0, Math.Max(0, width - 5));

        var head = IsLegacy ? "┌─" : "╭─";
        if (text.Length > 0) head += " " + text + " ";

        return head + '─'.Repeat(Math.Max(0, width - head.Length - 1)) + (IsLegacy ? "┐" : "╮");
    }

    /// <summary>Bottom border of the input box.</summary>
    private static string BottomBorder(int width)
    {
        var inner = Math.Max(0, width - 2);
        return IsLegacy ? "└" + '─'.Repeat(inner) + "┘" : "╰" + '─'.Repeat(inner) + "╯";
    }

    /// <summary>
    /// Draws the rows at their exact position. Every row is filled up to the width of the chat, so no
    /// character of a previous render is left behind. The console is never scrolled because no row
    /// reaches the last column of the window.
    /// </summary>
    public static void Draw(IReadOnlyList<ChatRow> rows, ChatLayout layout, bool fullRedraw)
    {
        var positionable = ConsoleHelpers.CanPositionCursor();

        try
        {
            if (fullRedraw && positionable)
            {
                System.Console.SetCursorPosition(0, 0);
                System.Console.Clear();
            }

            foreach (var row in rows)
            {
                if (row.Row < 0 || row.Row >= layout.Height) continue;

                if (positionable) System.Console.SetCursorPosition(0, row.Row);
                WriteRow(row.Line, layout.Width);

                // Without a cursor that can be moved around there is no screen to update,
                // every render prints the whole chat once more.
                if (!positionable) System.Console.WriteLine();
            }
        }
        catch (IOException)
        {
            // The console went away (redirected or closed), there is nothing left to draw on.
        }
        catch (ArgumentOutOfRangeException)
        {
            // The window shrank while the chat was drawn, the next render uses the new size.
        }
        finally
        {
            System.Console.ResetColor();
        }
    }

    /// <summary>Writes one row with the colors of its pieces and fills the rest of the row.</summary>
    private static void WriteRow(ChatLine line, int width)
    {
        foreach (var span in line.Spans)
        {
            ConsoleHelpers.ApplyColors(span.Foreground, span.Background);
            System.Console.Write(span.Text);
        }

        var fill = width - line.Length;
        if (fill <= 0) return;

        // The background of a message covers the whole row, the padding uses the colors of its text.
        var tail = line.Tail;
        ConsoleHelpers.ApplyColors(tail.Foreground, tail.Background);
        System.Console.Write(' '.Repeat(fill));
    }
}
