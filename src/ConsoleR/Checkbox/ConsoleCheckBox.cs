using ConsoleR.Checkbox.Models;

namespace ConsoleR;

public static partial class Console {
    public static ConsoleCheckbox Checkbox(string displayText, params string[] options) {
        return new ConsoleCheckbox(displayText, true,true,true, options);
    }

    public static ConsoleCheckbox Checkbox(string displayText, bool multiselect, params string[] options)
    {
        return new ConsoleCheckbox(displayText, true, true, multiselect, options);
    }

    public static ConsoleCheckbox Checkbox(string displayText, bool multiselect, bool selectFirst, params string[] options)
    {
        return new ConsoleCheckbox(displayText, selectFirst,true, multiselect, options);
    }
}

public class ConsoleCheckbox
{
    private List<CheckboxOptions> _options;
    private int _hoveredIndex;
    private int _selectedIndex;
    private string _displayText;
    private readonly bool _multiSelect;
    private readonly bool _required;
    private bool _error;
    private ConsoleKey _key;
    private ConsoleKey _prevKey;

    public ConsoleCheckbox(string displayText, bool selectFirst = false, params string[] options)
    {
        _multiSelect = false;
        _required = true;
        _options = [];
        _displayText = displayText;
        Init(selectFirst, options);
    }

    public ConsoleCheckbox(string displayText, bool multiMode, bool required, bool selectFirst = false, params string[] options)
    {
        _multiSelect = multiMode;
        _required = required;
        _options = [];
        _displayText = displayText;
        Init(selectFirst, options);
    }

    private void Init(bool selectFirst, string[] options)
    {
        _hoveredIndex = 0;
        _selectedIndex = selectFirst ? 0 : -1;
        _error = false;

        _options = [];

        for (var i = 0; i < options.Length; i++)
            _options.Add(new CheckboxOptions(options[i], _selectedIndex == i, i == _hoveredIndex, i));
    }

    private CheckboxReturn[] ReturnData()
    {
        var list = new List<CheckboxReturn>();
        foreach (var option in _options)
        {
            if (option.Selected) list.Add(option.GetData());
        }

        return list.ToArray();
    }

    public void Show()
    {
        System.Console.Clear();
        System.Console.WriteLine(_displayText);
        System.Console.WriteLine("(Use Arrow keys to navigate up and down, Space bar to select and Enter to submit)");

        foreach (var option in _options)
        {
            System.Console.ForegroundColor = option.Selected
                ? (option.Hovered ? ConsoleColor.DarkGreen : ConsoleColor.Green)
                : (option.Hovered ? ConsoleColor.White : ConsoleColor.DarkGray);

            System.Console.WriteLine((option.Selected ? "(*) " : "( ) ") + $"{option.Option}");
        }
        System.Console.ResetColor();
        if (_error) System.Console.WriteLine("\nAt least one item has to be selected!");
    }

    public CheckboxReturn[] Select()
    {
        Show();
        var end = false;
        while (!end)
        {
            _key = System.Console.KeyAvailable ? System.Console.ReadKey(true).Key : ConsoleKey.D9;
            if (_key == _prevKey) continue;
            _options[_hoveredIndex].Hovered = false;

            switch (_key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    _hoveredIndex = _hoveredIndex - 1 >= 0 ? _hoveredIndex - 1 : _options.Count - 1;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    _hoveredIndex = _hoveredIndex + 1 < _options.Count ? _hoveredIndex + 1 : 0;
                    break;

                case ConsoleKey.Spacebar:
                    _options[_hoveredIndex].Selected = !_options[_hoveredIndex].Selected;
                    if (!_multiSelect)
                    {
                        if (_selectedIndex > -1 && _hoveredIndex != _selectedIndex)
                            _options[_selectedIndex].Selected = false;
                        _selectedIndex = _hoveredIndex;
                    }

                    _error = false;
                    break;

                case ConsoleKey.Enter:
                    if (_required)
                    {
                        if (_options.Any(option => option.Selected))
                        {
                            end = true;
                        }

                        if (!end) _error = true;
                    }
                    else end = true;

                    break;
            }

            _options[_hoveredIndex].Hovered = true;
            Show();
            _prevKey = _key;
        }

        return ReturnData();
    }
}

