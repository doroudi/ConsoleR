﻿using ConsoleR;
using Console = ConsoleR.Console;

string[] frontEndFrameworks = ["Blazor", "Angular", "Vue", "React", "VanillaJs"];
var selectedItem = Console.Menu("Please Select One beloved frontend framework", frontEndFrameworks).Select();
Console.AsciiArt(frontEndFrameworks[selectedItem], GetFrameworkColor(frontEndFrameworks[selectedItem]));

Console.Info("Progress started...");
Console.Warning("It seems there is issue in the system");
Console.Error("Process failed :(");
Console.Info("Retrying...");
Console.ReadLine("Press enter to continue");
Console.Success("Progress Succeed", showIcon: true);
Console.WriteLine("Wait it is not completed yet", ConsoleColor.Magenta);

var password = Console.Password("Enter your password:");
Console.Alert($"your password is: {password}", "Password", ConsoleMessageType.Info);


string[] plugins = ["TypeScript", "Linter", "Nuxt", "Vite"];
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