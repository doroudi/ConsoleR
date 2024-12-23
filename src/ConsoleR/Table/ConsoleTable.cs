namespace ConsoleR;

public partial class Console {
    public static void Table<T>(IEnumerable<T> data) {
        var properties = typeof(T).GetProperties();
        var headers = properties.Select(p => p.Name).ToArray();
        var values = data.Select(d => properties.Select(p => p.GetValue(d)).ToArray()).ToArray();
        PrintTable(headers, values);
    }

    private static void PrintTable(string[] headers, object[][] values) {
        var columnWidths = new int[headers.Length];
        for (int i = 0; i < headers.Length; i++) {
            columnWidths[i] = headers[i].Length;
            for (int j = 0; j < values.Length; j++) {
                var value = values[j][i].ToString();
                columnWidths[i] = Math.Max(columnWidths[i], value.Length);
            }
        }

        var header = string.Join(" | ", headers.Select((h, i) => h.PadRight(columnWidths[i])));
        WriteLine(header);
        WriteLine(string.Join('-', columnWidths.Select(w => new string('-', w))));
        foreach (var row in values) {
            var line = string.Join(" | ", row.Select((v, i) => v.ToString().PadRight(columnWidths[i])));
            WriteLine(line);
        }
    }
}