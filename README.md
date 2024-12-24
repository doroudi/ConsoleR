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
