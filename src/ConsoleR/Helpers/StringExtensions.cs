namespace ConsoleR;

public static class StringExtensions
{
    public static string Repeat(this char input, int count)
    {
        return new string(input, count);
    }
}