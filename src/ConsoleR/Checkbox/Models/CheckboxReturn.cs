namespace ConsoleR.Checkbox.Models;

public class CheckboxOptions(string option, bool selected, bool hovered, int index)
{
    public bool Selected { get; set; } = selected;
    public bool Hovered { get; set; } = hovered;
    public string Option => option;

    public CheckboxReturn GetData()
    {
        return new CheckboxReturn(index, option);
    }
}

public class CheckboxReturn(int index, string option)
{
    public int Index => index;
    public string Option => option;
}
