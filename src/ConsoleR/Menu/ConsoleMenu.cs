using ConsoleR.Menu.Models;

namespace ConsoleR.Menu;

public class ConsoleMenu
{
    private string _displayText;
    private int _selectedIndex;
    private bool _error;
    private List<MenuOption> _options;
    private ConsoleKey _key;
    private ConsoleKey _prevKey;

    public ConsoleMenu(string displayText, bool selectFirst = true, params string[] options)
    {
        _options = [];
        _displayText = displayText;
        Init(selectFirst, options);
    }


    private void Init(bool selectFirst, string[] options)
    {
        _selectedIndex = selectFirst ? 0 : -1;
        _error = false;

        for (var i = 0; i < options.Length; i++)
            _options.Add(new MenuOption(options[i], _selectedIndex == i));
    }

    public void Show()
    {
        Console.Clear();
        Console.WriteLine(_displayText);
        Console.WriteLine("(Use Arrow keys to navigate up and down to select and Enter to submit)");

        foreach (var option in _options)
        {
            Console.ForegroundColor = option.Selected ? ConsoleColor.Green : ConsoleColor.White;
            Console.WriteLine((option.Selected ? "[*] " : "[ ] ") + $"{option.Option}");
        }
        Console.ResetColor();
        //if (_error) Console.WriteLine("\nAt least one item has to be selected!");
    }


    public int Select()
    {
        Show();
        var end = false;
        while (!end)
        {
            _key = Console.KeyAvailable ? Console.ReadKey(true).Key : ConsoleKey.D9;
            if (_key == _prevKey) continue;
            _options[_selectedIndex].Selected = false;

            switch (_key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    _selectedIndex = _selectedIndex - 1 >= 0 ? _selectedIndex - 1 : _options.Count - 1;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    _selectedIndex = _selectedIndex + 1 < _options.Count ? _selectedIndex + 1 : 0;
                    break;

                case ConsoleKey.Enter:
                    end = true;

                    break;
            }

            _options[_selectedIndex].Selected = true;
            Show();
            _prevKey = _key;
        }
        return _selectedIndex;
    }
}



