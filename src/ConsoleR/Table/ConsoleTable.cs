namespace ConsoleR;

public partial class Console {
    public static void Table<T>(IEnumerable<T> data, ConsoleColor? borderColor = null) {
        var properties = typeof(T).GetProperties();
        var headers = properties.Select(p => p.Name).ToArray();
        var values = data.Select(d => properties.Select(p => p.GetValue(d)).ToArray()).ToArray();
        PrintTable(headers, values, borderColor);
    }

    private static void PrintTable(string[] headers, object[][] values, ConsoleColor? borderColor = null) {
        var columnWidths = new int[headers.Length];
        for (int i = 0; i < headers.Length; i++) {
            columnWidths[i] = headers[i].Length;
            for (int j = 0; j < values.Length; j++) {
                var value = values[j][i].ToString();
                columnWidths[i] = Math.Max(columnWidths[i], value.Length);
            }
        }

        var maxLength = columnWidths.Sum() + columnWidths.Length * 3 - 1;
        WriteLine($"┌{'─'.Repeat(maxLength)}┐", borderColor);
        var header = string.Join(" │ ", headers.Select((h, i) => h.PadRight(columnWidths[i])));
        PrintWithBorder(header, borderColor);
        PrintWithBorder('─'.Repeat(maxLength - 2), borderColor, true);
        foreach (var row in values) {
            var line = string.Join(" │ ", row.Select((v, i) => v.ToString().PadRight(columnWidths[i])));
            PrintWithBorder(line, borderColor);
        }
        WriteLine($"└{'─'.Repeat(maxLength)}┘", borderColor);
    }

    private static void PrintWithBorder(string line, ConsoleColor? color = null, bool fillColor = false) {
        Write($"│ ", color);
        Write(line, fillColor? color : null);
        WriteLine($" │", color);
    }
}