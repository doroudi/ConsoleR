namespace ConsoleR;

public static class StringExtensions
{
    public static string Repeat(this char input, int count)
    {
        return new string(input, count);
    }

    public static string Fill(this string input, int count, char fillWith = ' ')
    {
        if(input.Length < count)
            return input + fillWith.Repeat(count - input.Length);

        return input;
    }

    public static string RemoveColorTags(this string input)
    {
        var regex = new System.Text.RegularExpressions.Regex(@"\[(.*?)\]");
        return regex.Replace(input, string.Empty);
    }
}