using static ConsoleR.Consoler;
const string message = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

AsciiArt("HELLO", ConsoleColor.Yellow);



WriteLine(message, ConsoleColor.Cyan);
Error("Error Occurred in app. please restart");
Success("Task Done successfully");
Info("Press any key to go next demo:");
Console.ReadKey();
string[] frontEndFrameworks = ["Blazor", "Angular", "Vue", "React", "JS"];
var selectedItem = Menu("Please Select One beloved frontend framework", frontEndFrameworks).Select();
Success("Your choice is: \n\n");
AsciiArt(frontEndFrameworks[selectedItem], GetFrameworkColor(frontEndFrameworks[selectedItem]));

Info("Ready for next demo? press any key");
ReadKey();


string[] plugins = ["Typescript", "Linter", "Nuxt", "Vite"];
var selectedItems = Checkbox("Select feature that you want to install:", plugins).Select();
for (int i = 0; i < selectedItems.Length; i++) {
    var plugin = selectedItems[i];
    WriteLine(plugin.Option, (ConsoleColor)i);
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