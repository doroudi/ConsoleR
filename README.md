# ConsoleR

ConsoleR is set of utilities to make awesome console apps in .Net

## Installation

```bash
dotnet add package Doroudi.ConsoleR
```

or

```bash
NuGet\Install-Package Doroudi.ConsoleR
```

### Features

- Menu
- CheckBox
- AsciiArt
- WriteLine
- Password
- Alert
- Table


#### How To Use

```csharp
using ConsoleR;
using ConsoleR.Menu.Models;
using Console = ConsoleR.Console;

// AsciiArt
Console.AsciiArt("ConsoleR", ConsoleColor.Yellow);
Console.ReadLine("Press enter to continue");

// WriteLine utilities
Console.Info("Progress started");
Console.Warning("It seems there is issue in the system");
Console.Error("Process failed :(");
Console.Info("Retrying...");
Console.Success("Progress Succeed", showIcon: true);
Console.WriteLine("Wait it is not completed yet", ConsoleColor.Magenta);

// Get Masked Password
var password = Console.Password("Enter your password:");

// Alert
Console.Alert($"your password is: {password}", "Password", ConsoleMessageType.Info);

// Console Menu
string[] frontEndFrameworks = ["Blazor", "Angular", "Vue", "React", "VanillaJs"];
var selectedItem = Console.Menu("Please Select One beloved frontend framework", frontEndFrameworks).Select();
Console.Success("Your choice is: \n\n");
Console.AsciiArt(frontEndFrameworks[selectedItem], ConsoleColor.Yellow);

// Menus with more options than the console window is high are scrollable:
// only the options that fit are shown, the box follows the selection while navigating.
string[] languages = ["C#", "Java", "Python", "JavaScript", "TypeScript", "Go", "Rust", "C++", "Kotlin", "Ruby",
    "Swift", "PHP", "Dart", "Elixir", "Scala", "Haskell", "F#", "Clojure", "Lua", "Perl", "R", "Julia", "Zig", "Nim"];

// The box is sized to the window height and scrolls when needed
var language = Console.Menu("Select your favorite programming language:", languages).Select();

// Or keep the box at a fixed size, five options at a time
var framework = Console.Menu("Select one frontend framework:", 5, frontEndFrameworks).Select();

// Everything is configurable through settings
var settings = new MenuSettings
{
    DisplayText = "Select a plugin:",
    VisibleItems = 5,          // null = as many options as fit into the window
    ShowScrollBox = true,      // null = box only when the options do not fit
    WrapAround = true,
    SelectedColor = ConsoleColor.Cyan,
    OptionColor = ConsoleColor.Gray
};
var selectedPlugin = Console.Menu(settings, ["Linter", "Formatter", "Test runner", "Bundler", "Dev server", "Docs"]).Select();


// Checkbox
string[] plugins = ["Typescript", "Linter", "Nuxt", "Vite"];
var selectedItems = Console.Checkbox("Select feature that you want to install:", plugins).Select();
for (int i = 0; i < selectedItems.Length; i++) {
    var plugin = selectedItems[i];
    Console.WriteLine(plugin.Option, (ConsoleColor)i);
}

// Table
Person[] people = [
    new Person("Saeid",30, "Tehran"),
    new Person("Saman", 25, "Marand"),
    new Person("Alice", 35, "Zurich"),
    new Person("Alex", 40, "Turin")
];

Console.Table(people);


record Person(string Name,int Age, string City);
```

#### Output

![ConsoleR](https://raw.githubusercontent.com/doroudi/ConsoleR/refs/heads/main/docs/ConsoleRBanner.png)
