using ConsoleR.Menu.Models;

namespace ConsoleR;

internal enum MenuRowKind
{
    Header,
    TopBorder,
    ItemsAbove,
    Item,
    ItemsBelow,
    BottomBorder
}

/// <summary>
/// One line of the rendered menu. Rows are built in the exact order they are drawn in.
/// </summary>
internal readonly struct MenuRow
{
    public MenuRow(string text, ConsoleColor? color, MenuRowKind kind, int optionIndex = -1)
    {
        Text = text;
        Color = color;
        Kind = kind;
        OptionIndex = optionIndex;
    }

    /// <summary>Full text of the line, already padded to the width of its row.</summary>
    public string Text { get; }

    /// <summary>Color of the line, <c>null</c> means the default console color.</summary>
    public ConsoleColor? Color { get; }

    public MenuRowKind Kind { get; }

    /// <summary>Index of the option of this row, <c>-1</c> for every non option row.</summary>
    public int OptionIndex { get; }
}

/// <summary>
/// Row positions and sizes of a single menu render. All rows are consecutive and start at the top row.
/// </summary>
internal readonly struct MenuLayout : IEquatable<MenuLayout>
{
    public MenuLayout(
        int headerRows,
        int headerWidth,
        int width,
        int topBorderRow,
        int itemsAboveRow,
        int itemsTop,
        int itemsBelowRow,
        int bottomBorderRow,
        int pageSize,
        int lastRow,
        bool showBox,
        bool showIndicators)
    {
        HeaderRows = headerRows;
        HeaderWidth = headerWidth;
        Width = width;
        TopBorderRow = topBorderRow;
        ItemsAboveRow = itemsAboveRow;
        ItemsTop = itemsTop;
        ItemsBelowRow = itemsBelowRow;
        BottomBorderRow = bottomBorderRow;
        PageSize = pageSize;
        LastRow = lastRow;
        ShowBox = showBox;
        ShowIndicators = showIndicators;
    }

    public int HeaderRows { get; }

    /// <summary>Width used by the display text and the hint, the full window width.</summary>
    public int HeaderWidth { get; }

    /// <summary>Width of the option box.</summary>
    public int Width { get; }

    public int TopBorderRow { get; }

    public int ItemsAboveRow { get; }

    public int ItemsTop { get; }

    public int ItemsBelowRow { get; }

    public int BottomBorderRow { get; }

    /// <summary>Number of options shown in this render.</summary>
    public int PageSize { get; }

    public int LastRow { get; }

    public bool ShowBox { get; }

    public bool ShowIndicators { get; }

    public int RowCount => LastRow + 1;

    public bool Equals(MenuLayout other) =>
        HeaderRows == other.HeaderRows &&
        HeaderWidth == other.HeaderWidth &&
        Width == other.Width &&
        TopBorderRow == other.TopBorderRow &&
        ItemsAboveRow == other.ItemsAboveRow &&
        ItemsTop == other.ItemsTop &&
        ItemsBelowRow == other.ItemsBelowRow &&
        BottomBorderRow == other.BottomBorderRow &&
        PageSize == other.PageSize &&
        LastRow == other.LastRow &&
        ShowBox == other.ShowBox &&
        ShowIndicators == other.ShowIndicators;

    public override bool Equals(object? obj) => obj is MenuLayout other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(HeaderRows, HeaderWidth, Width, ItemsTop, PageSize, LastRow, ShowBox, ShowIndicators);
}

/// <summary>
/// Turns the options into the lines of a scrollable menu box and draws them on the console.
/// </summary>
internal static class MenuRenderer
{
    internal const string HintWithNumbers = "(Use Arrow keys to navigate up and down to select and Enter to submit or use number to select)";
    internal const string Hint = "(Use Arrow keys to navigate up and down to select and Enter to submit)";

    /// <summary>Rows that stay free at the bottom of the window, so the cursor can leave the box.</summary>
    private const int BottomMarginRows = 1;

    private const int FallbackWindowWidth = 80;
    private const int FallbackWindowHeight = 25;

    private static bool IsLegacy => ConsoleHelpers.IsLegacy;

    private static string UpSign => IsLegacy ? "^" : "▲";

    private static string DownSign => IsLegacy ? "v" : "▼";

    private static string Ellipsis => IsLegacy ? ".." : "…";

    /// <summary>Lines of the display text.</summary>
    public static string[] GetHeaderLines(string? displayText)
    {
        var text = displayText ?? string.Empty;
        return text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
    }

    /// <summary>
    /// All header rows of the menu: the display text and the hint, each wrapped to the given width so
    /// that no header line can push the option rows down.
    /// </summary>
    public static List<string> BuildHeaderLines(string? displayText, bool showNumbers, int width)
    {
        var lines = new List<string>();

        foreach (var line in GetHeaderLines(displayText))
            lines.AddRange(WrapText(line, width));

        lines.AddRange(WrapText(showNumbers ? HintWithNumbers : Hint, width));

        return lines;
    }

    /// <summary>Width that the display text and the hint may use inside a window of the given width.</summary>
    public static int GetHeaderWidth(int windowWidth) => Math.Max(8, windowWidth - 1);

    /// <summary>Breaks a text into lines that are not longer than the given width.</summary>
    public static List<string> WrapText(string text, int width)
    {
        var lines = new List<string>();
        if (width < 1) width = 1;

        var current = new System.Text.StringBuilder();

        foreach (var word in text.Split(' '))
        {
            var candidate = current.Length == 0 ? word : current + " " + word;

            if (candidate.Length <= width)
            {
                current.Clear();
                current.Append(candidate);
                continue;
            }

            if (current.Length > 0)
            {
                lines.Add(current.ToString());
                current.Clear();
            }

            // A single word can be longer than a whole line, break it apart.
            var remaining = word;
            while (remaining.Length > width)
            {
                lines.Add(remaining.Substring(0, width));
                remaining = remaining.Substring(width);
            }

            current.Append(remaining);
        }

        if (current.Length > 0 || lines.Count == 0) lines.Add(current.ToString());

        return lines;
    }

    /// <summary>Size of the console window, with a sane fallback for redirected output.</summary>
    public static (int Width, int Height) GetWindowSize()
    {
        try
        {
            var width = System.Console.WindowWidth;
            var height = System.Console.WindowHeight;
            if (width > 0 && height > 0) return (width, height);
        }
        catch (IOException) { }
        catch (PlatformNotSupportedException) { }
        catch (ArgumentOutOfRangeException) { }

        return (FallbackWindowWidth, FallbackWindowHeight);
    }

    /// <summary>Positioning the cursor requires a real, not redirected, output.</summary>
    public static bool CanPositionCursor()
    {
        try
        {
            return !System.Console.IsOutputRedirected;
        }
        catch (IOException)
        {
            return false;
        }
    }

    /// <summary>Text of a single option, including the selection sign and the optional number.</summary>
    public static string GetItemText(string optionText, int index, bool selected, bool showNumbers)
    {
        var numberSign = showNumbers ? $"{index + 1}." : string.Empty;
        var selectedSign = selected ? (IsLegacy ? ">" : "●") : " ";
        return $"{selectedSign} {numberSign} {optionText}";
    }

    /// <summary>
    /// Calculates how many options are visible and where every row of the menu is placed.
    /// Only as many options as fit into the window are shown; the rest is reached by scrolling.
    /// <paramref name="headerLines"/> are the already wrapped header rows, built with
    /// <see cref="BuildHeaderLines"/> for <see cref="GetHeaderWidth"/>, every one of them uses a row.
    /// </summary>
    public static MenuLayout ComputeLayout(
        IReadOnlyList<string> headerLines,
        IReadOnlyList<string> optionTexts,
        int windowWidth,
        int windowHeight,
        bool showNumbers,
        MenuSettings settings)
    {
        var maxWidth = GetHeaderWidth(windowWidth);
        var headerRows = headerLines?.Count ?? 0;
        var texts = optionTexts ?? Array.Empty<string>();
        var itemCount = texts.Count;

        var requested = settings.VisibleItems.GetValueOrDefault();
        var desired = requested > 0 ? Math.Min(itemCount, requested) : itemCount;
        var available = Math.Max(1, windowHeight - headerRows - BottomMarginRows);

        var indicatorsAllowed = true;
        var boxAllowed = true;

        // The box border and the scroll indicators have to give way when the window is too short
        // for them and at least one option, an option on screen is worth more than the decoration.
        if (available - (indicatorsAllowed ? 2 : 0) - (boxAllowed ? 2 : 0) < 1)
        {
            boxAllowed = false;
            if (available - (indicatorsAllowed ? 2 : 0) < 1) indicatorsAllowed = false;
        }

        var pageSize = itemCount > 0 ? Math.Max(1, Math.Min(desired, available)) : 0;
        var scroll = pageSize < itemCount;
        var showIndicators = indicatorsAllowed && scroll;
        var showBox = boxAllowed && (settings.ShowScrollBox ?? false);

        // The scroll indicators and the box border need rows of their own, which can reduce the page size
        // and with it the need for scrolling. Repeat until the layout is stable.
        for (var attempt = 0; attempt < 4; attempt++)
        {
            var reserved = (showIndicators ? 2 : 0) + (showBox ? 2 : 0);
            var fit = Math.Max(1, available - reserved);
            var size = itemCount > 0 ? Math.Max(1, Math.Min(desired, fit)) : 0;
            var nextScroll = size < itemCount;
            var nextIndicators = indicatorsAllowed && nextScroll;
            var nextBox = boxAllowed && (settings.ShowScrollBox ?? nextScroll);
            var stable = size == pageSize && nextScroll == scroll && nextBox == showBox && nextIndicators == showIndicators;

            pageSize = size;
            scroll = nextScroll;
            showIndicators = nextIndicators;
            showBox = nextBox;

            if (stable) break;
        }

        var contentWidth = 0;
        for (var i = 0; i < itemCount; i++)
            contentWidth = Math.Max(contentWidth, GetItemText(texts[i], i, false, showNumbers).Length);

        if (showIndicators)
        {
            var digits = itemCount.ToString().Length;
            contentWidth = Math.Max(contentWidth, UpSign.Length + 1 + digits + " more above".Length);
            contentWidth = Math.Max(contentWidth, DownSign.Length + 1 + digits + " more below".Length);
        }

        if (contentWidth <= 0) contentWidth = 1;

        var width = showBox
            ? Math.Min(maxWidth, Math.Max(8, contentWidth + 4))
            : Math.Min(maxWidth, contentWidth);
        if (width < 1) width = 1;

        var row = headerRows;
        var topBorderRow = -1;
        var itemsAboveRow = -1;
        var itemsBelowRow = -1;
        var bottomBorderRow = -1;

        if (showBox) topBorderRow = row++;
        if (showIndicators) itemsAboveRow = row++;

        var itemsTop = row;
        row += pageSize;

        if (showIndicators) itemsBelowRow = row++;
        if (showBox) bottomBorderRow = row++;

        return new MenuLayout(
            headerRows,
            maxWidth,
            width,
            topBorderRow,
            itemsAboveRow,
            itemsTop,
            itemsBelowRow,
            bottomBorderRow,
            pageSize,
            row - 1,
            showBox,
            showIndicators);
    }

    /// <summary>
    /// Builds every line of the menu in drawing order. The header lines have to be the same ones
    /// that were handed to <see cref="ComputeLayout"/>, otherwise the option rows would shift.
    /// </summary>
    public static IReadOnlyList<MenuRow> BuildRows(
        IReadOnlyList<string> headerLines,
        IReadOnlyList<string> optionTexts,
        int selectedIndex,
        bool showNumbers,
        MenuSettings settings,
        MenuLayout layout,
        MenuViewport viewport)
    {
        var texts = optionTexts ?? Array.Empty<string>();
        var rows = new List<MenuRow>(Math.Max(0, layout.RowCount));

        foreach (var line in headerLines ?? Array.Empty<string>())
            rows.Add(new MenuRow(Fit(line, layout.HeaderWidth), null, MenuRowKind.Header));

        if (layout.ShowBox)
            rows.Add(new MenuRow(BorderLine(layout.Width, top: true), ConsoleColor.DarkGray, MenuRowKind.TopBorder));

        if (layout.ItemsAboveRow >= 0)
            rows.Add(new MenuRow(
                ContentRow(viewport.HasItemsAbove ? $"{UpSign} {viewport.ItemsAbove} more above" : string.Empty, layout),
                ConsoleColor.DarkGray,
                MenuRowKind.ItemsAbove));

        for (var i = 0; i < layout.PageSize; i++)
        {
            var index = viewport.Offset + i;
            if (index < 0 || index >= texts.Count) break;

            var selected = index == selectedIndex;
            rows.Add(new MenuRow(
                ContentRow(GetItemText(texts[index], index, selected, showNumbers), layout),
                selected ? settings.SelectedColor : settings.OptionColor,
                MenuRowKind.Item,
                index));
        }

        if (layout.ItemsBelowRow >= 0)
            rows.Add(new MenuRow(
                ContentRow(viewport.HasItemsBelow ? $"{DownSign} {viewport.ItemsBelow} more below" : string.Empty, layout),
                ConsoleColor.DarkGray,
                MenuRowKind.ItemsBelow));

        if (layout.ShowBox)
            rows.Add(new MenuRow(BorderLine(layout.Width, top: false), ConsoleColor.DarkGray, MenuRowKind.BottomBorder));

        return rows;
    }

    /// <summary>
    /// Draws the rows at their exact position. Every line is padded to the width of its row so that
    /// no character of a previous render is left behind. The console is never scrolled because no
    /// line reaches the last column of the window.
    /// </summary>
    public static void Draw(IReadOnlyList<MenuRow> rows, bool fullRedraw, int windowHeight)
    {
        if (!CanPositionCursor())
        {
            // Without a cursor to move around there is nothing to update, print the menu once.
            if (!fullRedraw) return;

            try
            {
                foreach (var row in rows)
                {
                    ApplyColor(row.Color);
                    System.Console.WriteLine(row.Text);
                }
            }
            finally
            {
                System.Console.ResetColor();
            }

            return;
        }

        try
        {
            if (fullRedraw)
            {
                System.Console.SetCursorPosition(0, 0);
                System.Console.Clear();
            }

            var drawable = DrawableRowCount(rows.Count, windowHeight);
            for (var i = 0; i < drawable; i++)
            {
                // The header never changes while navigating, only a full redraw has to rewrite it.
                if (!fullRedraw && rows[i].Kind == MenuRowKind.Header) continue;

                System.Console.SetCursorPosition(0, i);
                ApplyColor(rows[i].Color);
                System.Console.Write(rows[i].Text);
            }
        }
        catch (IOException)
        {
            // The console went away (redirected or closed), there is nothing left to draw on.
        }
        finally
        {
            System.Console.ResetColor();
        }
    }

    /// <summary>
    /// Number of rows that can be drawn in a window of the given height. Rows that do not fit are
    /// skipped instead of scrolling the window.
    /// </summary>
    public static int DrawableRowCount(int rowCount, int windowHeight)
    {
        if (windowHeight <= 0) windowHeight = FallbackWindowHeight;
        return Math.Max(0, Math.Min(rowCount, windowHeight));
    }

    private static void ApplyColor(ConsoleColor? color)
    {
        if (color.HasValue) System.Console.ForegroundColor = color.Value;
        else System.Console.ResetColor();
    }

    /// <summary>Line of the option box, with the border signs when the box is visible.</summary>
    private static string ContentRow(string content, MenuLayout layout)
    {
        if (!layout.ShowBox) return Fit(content, layout.Width);

        return "│ " + Fit(content, Math.Max(0, layout.Width - 4)) + " │";
    }

    private static string BorderLine(int width, bool top)
    {
        var inner = Math.Max(0, width - 2);
        var horizontal = new string('─', inner);

        if (IsLegacy) return top ? "┌" + horizontal + "┐" : "└" + horizontal + "┘";
        return top ? "╭" + horizontal + "╮" : "╰" + horizontal + "╯";
    }

    /// <summary>Cuts the text to the given width and fills it up with spaces.</summary>
    private static string Fit(string text, int width)
    {
        if (width <= 0) return string.Empty;
        if (text.Length > width)
        {
            var ellipsis = Ellipsis;
            text = width <= ellipsis.Length
                ? text.Substring(0, width)
                : text.Substring(0, width - ellipsis.Length) + ellipsis;
        }

        return text.Length < width ? text.PadRight(width) : text;
    }
}
