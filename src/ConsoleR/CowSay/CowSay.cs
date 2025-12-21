namespace ConsoleR;

public partial class Console
{
    public static void CowSay(string message, string cowEye = "oo")
    {
        var lines = message.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
        string output = string.Empty;
        if (lines.Count() == 1)
            output = FormatSingleLine(message);
        else if(lines.Count() > 1)
            output = FormatMultiLine(message);

        if (cowEye.Length < 2)
            cowEye = "oo";

        string cowSay = @$"{output}
        \   ^__^
         \  ({cowEye[..2]})\_______
            (__)\       )\/\
                ||----w |
                ||     ||
        ";

        WriteLine(cowSay);
    }

    private static string FormatSingleLine(string message)
    {
        var len = message.RemoveColorTags().Length;
        string formatted =
$"  {'─'.Repeat(len)} \n" +
$"< {message} > \n" +
$"  {'─'.Repeat(len)}";

        return formatted;
    }

    private static string FormatMultiLine(string message)
    {
        var lines = message.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
        var maxLen = lines.Select(x=>x.RemoveColorTags()).Max(x => x.Length);
        string output = $"  {'─'.Repeat(maxLen)} \n";
        var index = 0;
        foreach (var line in lines)
        {
            if (index == 0)
                output += $@"/ {line.Fill(maxLen)} \ " + "\n";
            else if(index < lines.Count() - 1)
                output += $@"| {line.Fill(maxLen)} |" + "\n";
            else if (index == lines.Count() - 1)
                output += @$"\ {line.Fill(maxLen)} /" + "\n";
            index++;
        }
        output += $"  {'─'.Repeat(maxLen)}";
        return output;
    }
}