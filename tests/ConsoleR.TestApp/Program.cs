using Console = ConsoleR.Console;
const string message = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

Console.AsciiArt("HELLO", ConsoleColor.Yellow);



Console.WriteLine(message, ConsoleColor.Cyan);
Console.Error("Error Occurred in app. please restart");
Console.Success("Task Done successfully");
Console.Info("Press any key to go next demo:");
Console.ReadKey();
string[] frontEndFrameworks = ["Blazor", "Angular", "Vue", "React", "JS"];
var selectedItem = Console.Menu("Please Select One beloved frontend framework", frontEndFrameworks).Select();
Console.Success("Your choice is: \n\n");
Console.AsciiArt(frontEndFrameworks[selectedItem], GetFrameworkColor(frontEndFrameworks[selectedItem]));

Console.Info("Ready for next demo? press any key");
Console.ReadKey();


string[] plugins = ["Typescript", "Linter", "Nuxt", "Vite"];
var selectedItems = Console.Checkbox("Select feature that you want to install:", plugins).Select();
for (int i = 0; i < selectedItems.Length; i++) {
    var plugin = selectedItems[i];
    Console.WriteLine(plugin.Option, (ConsoleColor)i);
}


ConsoleColor GetFrameworkColor(string framework) {
    return framework switch {
        "Blazor" => ConsoleColor.DarkMagenta,
        "Angular" => ConsoleColor.Red,
        "Vue" => ConsoleColor.Green,
        "React" => ConsoleColor.Blue,
        "JS" => ConsoleColor.Yellow,
        _ => ConsoleColor.White
    };
}