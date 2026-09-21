using ConsoleR.Menu.Models;

namespace ConsoleR;

public static partial class Console
{
    public static ConsoleMenu Menu(string displayText, params string[] args)
    {
        return ConsoleMenu.Create(displayText, args);
    }

    public static ConsoleMenu Menu(string displayText, bool showNumbers, params string[] args)
    {
        return ConsoleMenu.Create(displayText, true, showNumbers, args);
    }

    /// <summary>
    /// Creates a menu that shows at most <paramref name="visibleItems"/> options at the same time
    /// and scrolls through the remaining ones while navigating.
    /// </summary>
    public static ConsoleMenu Menu(string displayText, int visibleItems, params string[] args)
    {
        return ConsoleMenu.Create(displayText, true, true, visibleItems, args);
    }

    public static ConsoleMenu Menu(string displayText, MenuSettings settings, params string[] args)
    {
        return ConsoleMenu.Create(displayText, settings, args);
    }

    public static ConsoleMenu Menu(MenuSettings settings, params string[] args)
    {
        return ConsoleMenu.Create(settings, args);
    }
}


public class ConsoleMenu
{
    private readonly List<MenuOption> _options;
    private readonly string[] _optionTexts;
    private readonly MenuSettings _settings;
    private readonly bool _showNumbers;
    private readonly MenuViewport _viewport;
    private int _selectedIndex;
    private MenuLayout _layout;
    private bool _hasRendered;

    public static ConsoleMenu Create(string displayText, params string[] options) => new(displayText, true, true, options);
    public static ConsoleMenu Create(string displayText, bool selectFirst = true, bool showNumbers = true, params string[] options) => new(displayText, selectFirst, showNumbers, options);

    /// <summary>
    /// Creates a menu that shows at most <paramref name="visibleItems"/> options at the same time.
    /// </summary>
    public static ConsoleMenu Create(string displayText, int visibleItems, params string[] options)
        => new(new MenuSettings { DisplayText = displayText, VisibleItems = visibleItems }, options);

    public static ConsoleMenu Create(string displayText, bool selectFirst, bool showNumbers, int visibleItems, params string[] options)
        => new(new MenuSettings { DisplayText = displayText, SelectFirst = selectFirst, ShowNumbers = showNumbers, VisibleItems = visibleItems }, options);

    public static ConsoleMenu Create(MenuSettings settings, params string[] options) => new(settings, options);

    public static ConsoleMenu Create(string displayText, MenuSettings settings, params string[] options)
    {
        var menuSettings = (settings ?? new MenuSettings()).Clone();
        menuSettings.DisplayText = displayText;
        return new ConsoleMenu(menuSettings, options);
    }

    private ConsoleMenu(string displayText, bool selectFirst, bool showNumbers, string[] options)
        : this(new MenuSettings { DisplayText = displayText, SelectFirst = selectFirst, ShowNumbers = showNumbers }, options)
    {
    }

    private ConsoleMenu(MenuSettings settings, string[] options)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;

        _settings = settings ?? new MenuSettings();
        _optionTexts = (string[])options.Clone();
        _showNumbers = _settings.ShowNumbers && options.Length < 10;
        _selectedIndex = _settings.SelectFirst && options.Length > 0 ? 0 : -1;

        _options = new List<MenuOption>(options.Length);
        for (var i = 0; i < options.Length; i++)
            _options.Add(new MenuOption(options[i], _selectedIndex == i));

        _viewport = new MenuViewport(_options.Count);
    }

    /// <summary>Draws the menu. The screen is cleared before the menu is printed.</summary>
    public void Show()
    {
        Render(fullRedraw: true);
        MoveCursorBelowMenu();
    }

    /// <summary>
    /// Shows the menu and waits for the user to select an option.
    /// Options that do not fit into the window are reached with the arrow keys, the box scrolls with the selection.
    /// </summary>
    /// <returns>Index of the selected option, <c>-1</c> when nothing was selected.</returns>
    public int Select()
    {
        var cursorVisible = true;
        try
        {
            cursorVisible = System.Console.CursorVisible;
            System.Console.CursorVisible = false;
        }
        catch (IOException) { }
        catch (PlatformNotSupportedException) { }

        try
        {
            Render(fullRedraw: true);

            var end = false;
            while (!end)
            {
                var key = System.Console.ReadKey(true).Key;
                end = ApplyKey(key);

                if (!end) Render(fullRedraw: false);
            }
        }
        finally
        {
            MoveCursorBelowMenu();

            try
            {
                if (cursorVisible) System.Console.CursorVisible = true;
            }
            catch (IOException) { }
            catch (PlatformNotSupportedException) { }
        }

        return _selectedIndex;
    }

    /// <summary>Applies a key press, returns <c>true</c> when the selection is submitted.</summary>
    private bool ApplyKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                Move(-1);
                return false;

            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                Move(1);
                return false;

            case ConsoleKey.PageUp:
                Move(-Math.Max(1, _viewport.PageSize));
                return false;

            case ConsoleKey.PageDown:
                Move(Math.Max(1, _viewport.PageSize));
                return false;

            case ConsoleKey.Home:
                if (_options.Count > 0) _selectedIndex = 0;
                return false;

            case ConsoleKey.End:
                if (_options.Count > 0) _selectedIndex = _options.Count - 1;
                return false;

            case ConsoleKey.Enter:
                return true;

            default:
                if (TryGetNumber(key, out var number) && number >= 1 && number <= _options.Count)
                {
                    _selectedIndex = number - 1;
                    return true;
                }

                return false;
        }
    }

    /// <summary>Moves the selection, the viewport follows the selection.</summary>
    private void Move(int offset)
    {
        if (_options.Count == 0) return;

        if (_selectedIndex < 0)
        {
            _selectedIndex = offset < 0 ? _options.Count - 1 : 0;
            return;
        }

        var next = _selectedIndex + offset;

        if (_settings.WrapAround)
            next = ((next % _options.Count) + _options.Count) % _options.Count;
        else
            next = Math.Min(Math.Max(next, 0), _options.Count - 1);

        _selectedIndex = next;
    }

    private static bool TryGetNumber(ConsoleKey key, out int number)
    {
        number = -1;

        if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
            number = (int)key - (int)ConsoleKey.D0;
        else if (key >= ConsoleKey.NumPad0 && key <= ConsoleKey.NumPad9)
            number = (int)key - (int)ConsoleKey.NumPad0;

        return number >= 0;
    }

    private void Render(bool fullRedraw)
    {
        var (windowWidth, windowHeight) = MenuRenderer.GetWindowSize();

        // Keep the option model in sync with the selection, it is rendered from _selectedIndex.
        for (var i = 0; i < _options.Count; i++)
            _options[i].Selected = i == _selectedIndex;

        // The header lines are built first, they decide how many rows are left for the options.
        var headerLines = MenuRenderer.BuildHeaderLines(_settings.DisplayText, _showNumbers, MenuRenderer.GetHeaderWidth(windowWidth));
        var layout = MenuRenderer.ComputeLayout(headerLines, _optionTexts, windowWidth, windowHeight, _showNumbers, _settings);

        _viewport.Resize(layout.PageSize);
        _viewport.ScrollTo(_selectedIndex);

        // Sizes change when the window is resized, in that case every row has to be drawn again.
        var redraw = fullRedraw || !_hasRendered || !layout.Equals(_layout);
        var rows = MenuRenderer.BuildRows(headerLines, _optionTexts, _selectedIndex, _showNumbers, _settings, layout, _viewport);

        MenuRenderer.Draw(rows, redraw, windowHeight);

        _layout = layout;
        _hasRendered = true;
    }

    private void MoveCursorBelowMenu()
    {
        if (!MenuRenderer.CanPositionCursor()) return;

        var (_, windowHeight) = MenuRenderer.GetWindowSize();
        var row = Math.Min(_layout.LastRow + 1, Math.Max(0, windowHeight - 1));

        try
        {
            System.Console.SetCursorPosition(0, row);
        }
        catch (IOException) { }
        catch (ArgumentOutOfRangeException) { }
    }
}
