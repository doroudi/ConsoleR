using ConsoleR.Checkbox.Models;
using System.Linq;

namespace ConsoleR;

public static partial class Console {
    public static ConsoleCheckbox Checkbox(string displayText, bool isRequired, params string[] options) {
        return ConsoleCheckbox.Create(displayText, isRequired, options);
    }

    public static ConsoleCheckbox Checkbox(string displayText, params string[] options)
    {
        return ConsoleCheckbox.Create(displayText, false, options);
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

    public static ConsoleCheckbox Create(string displayText, bool isRequired, params string[] options) => new(displayText, isRequired, options);
    public static ConsoleCheckbox Create(string displayText, params string[] options) 
        => new(displayText, true, options);
    private ConsoleCheckbox(string displayText, params string[] options)
    {
        _multiSelect = true;
        _required = false;
        _options = [];
        _displayText = displayText;
        Init(options);
    }

    private ConsoleCheckbox(string displayText, bool required, params string[] options)
    {
        _required = required;
        _multiSelect = true;
        _options = [];
        _displayText = displayText;
        Init(options);
    }

    private void Init(string[] options)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        _hoveredIndex = 0;
        _selectedIndex = -1;
        _error = false;

        _options = [];

        for (var i = 0; i < options.Length; i++)
            _options.Add(new CheckboxOptions(options[i], _selectedIndex == i, i == _hoveredIndex, i));
    }

    private CheckboxReturn[] ReturnData()
    {
        return _options.Where(option => option.Selected).Select(option => option.GetData()).ToArray();
    }

    public void Show()
    {
        System.Console.Clear();
        System.Console.WriteLine(_displayText);
        System.Console.WriteLine("(Use Arrow keys to navigate up and down, Space bar to select and Enter to submit)" + Environment.NewLine);

        foreach (var option in _options)
        {
            System.Console.ForegroundColor = option.Selected
                ? (option.Hovered ? ConsoleColor.DarkGreen : ConsoleColor.Green)
                : (option.Hovered ? ConsoleColor.White : ConsoleColor.DarkGray);

            System.Console.WriteLine((option.Selected ? "■ " : "□ ") + $"{option.Option}");
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

