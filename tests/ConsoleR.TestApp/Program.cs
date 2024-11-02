using ConsoleR;
const string message = "Hello, World!";


Consoler.Banner("This is ConsoleR, Build Awesome Console Apps with .Net", ConsoleColor.DarkGreen);
Consoler.ReadKey();


Consoler.WriteLine(message, ConsoleColor.Cyan);
Consoler.Error("Error Occurred in app. please restart");
Consoler.Success("Task Done successfully");
Consoler.Info("Press any key to go next demo:");
Console.ReadKey();
string[] frontEndFrameworks = ["Blazor", "Angular", "Vue", "React", "JS"];
var selectedItem = Consoler.Menu("Please Select One beloved frontend framework", frontEndFrameworks).Select();
Consoler.Success("Your choice is: \n\n");
Consoler.AsciiArt(frontEndFrameworks[selectedItem], GetFrameworkColor(frontEndFrameworks[selectedItem]));

Consoler.Info("Ready for next demo? press any key");
Consoler.ReadKey();


string[] plugins = ["Typescript", "Linter", "Nuxt", "Vite"];
var selectedItems = Consoler.Checkbox("Select feature that you want to install:", plugins).Select();
for (int i = 0; i < selectedItems.Length; i++) {
    var plugin = selectedItems[i];
    Consoler.WriteLine(plugin.Option, (ConsoleColor)i);
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