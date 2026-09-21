namespace ConsoleR.Menu.Models;

/// <summary>
/// Configuration of a console menu (<see cref="ConsoleR.ConsoleMenu"/>).
/// </summary>
public class MenuSettings
{
    /// <summary>
    /// Text printed above the menu items. A menu created from a display text overrides this value.
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Select the first option as soon as the menu is created. Default is <c>true</c>.
    /// </summary>
    public bool SelectFirst { get; set; } = true;

    /// <summary>
    /// Print the number of each option in front of it. Numbers are not printed when more than 9 options are provided.
    /// </summary>
    public bool ShowNumbers { get; set; } = true;

    /// <summary>
    /// Maximum number of options that are visible at the same time (the size of the scroll box).
    /// When <c>null</c> (default) the box is sized to the height of the console window and only as many
    /// options as fit into the window are shown.
    /// </summary>
    public int? VisibleItems { get; set; }

    /// <summary>
    /// Draw a border around the scrollable option box.
    /// When <c>null</c> (default) the border is drawn only when the options do not fit into the console window.
    /// </summary>
    public bool? ShowScrollBox { get; set; }

    /// <summary>
    /// Jump to the last option when moving up from the first one and to the first option when moving down
    /// from the last one. Default is <c>true</c>.
    /// </summary>
    public bool WrapAround { get; set; } = true;

    /// <summary>
    /// Color of the currently selected option.
    /// </summary>
    public ConsoleColor SelectedColor { get; set; } = ConsoleColor.Green;

    /// <summary>
    /// Color of the options that are not selected.
    /// </summary>
    public ConsoleColor OptionColor { get; set; } = ConsoleColor.White;

    internal MenuSettings Clone()
    {
        return new MenuSettings
        {
            DisplayText = DisplayText,
            SelectFirst = SelectFirst,
            ShowNumbers = ShowNumbers,
            VisibleItems = VisibleItems,
            ShowScrollBox = ShowScrollBox,
            WrapAround = WrapAround,
            SelectedColor = SelectedColor,
            OptionColor = OptionColor
        };
    }
}
