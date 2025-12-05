using ConsoleR;
using ConsoleR.Loading;
using System.Drawing;
using Console = ConsoleR.Console;

var outputText = "Welcome to ConsoleR!";

Console.AsciiArt("Output Color", ConsoleColor.Yellow);
Thread.Sleep(750);
Console.Write("Hello ", "#FFCC00");
Console.Write("World", ConsoleColor.Green);
Console.Write("; \n", Color.BlueViolet);
Console.WriteLine(outputText);
Console.WriteLine(outputText.ToString(), Color.Magenta);
Console.WriteLine(outputText.ToString(), Color.Cyan);
Console.WriteLine(outputText.ToString(), Color.GreenYellow);
Console.WriteLine(outputText.ToString(), "#FFCC00");
Console.WriteLine(outputText.ToString(), "#FF0000");
Console.WriteLine(outputText.ToString(), ConsoleColor.DarkGreen);
Console.WriteLine(outputText.ToString(), Color.Tan);
Console.ReadKey("Press any key to continue");
Console.Clear();
Console.AsciiArt("ALERT", ConsoleColor.Green);
Thread.Sleep(750);
Console.Alert($"Welcome to ConsoleR Test Application {Environment.NewLine} This is a tutorial to show features of this library", " ConsoleR ", MessageType.Info);
Console.Alert($"This is Warning", " Warning ", MessageType.Warning);
Console.Alert($"Error message goes here", " ERROR ", MessageType.Error);
Console.Alert($"It can be a successfull message also :)", "", MessageType.Success);
Console.ReadKey("\nPress any key to continue");
Console.Clear();
Console.AsciiArt("MENU", ConsoleColor.Green);
Thread.Sleep(750);
Console.Menu("Select your favorite programming language:", "C#", "Java", "Python", "JavaScript", "Go", "Rust", "C++", "Kotlin", "Ruby").Select();


var content = Console.ReadInBox("You:");
Console.Info($"Bot: {content}");
content = Console.ReadInBox("You:", ConsoleColor.Yellow);
var spinner = new Spinner();
await spinner.Start(() =>
{
    int x = 0;
    while (x < 10)
    {
        x++;
        Thread.Sleep(100);
    }
    throw new Exception("Something went wrong");
}, "Starting app");

var spinner2 = new Spinner();
await spinner2.Start(() =>
{
    int x = 0;
    while (x < 10)
    {
        x++;
        Thread.Sleep(100);
    }
    spinner2.SetText("Almost done...");
}, "Restart app");

Console.AsciiArt("NEXT TOP ", ConsoleColor.Green);
Console.AsciiArt("ConsoleR", ConsoleColor.Cyan);
Console.WriteLine("\nPress any key to continue");
Console.ReadKey();

string[] frontEndFrameworks = ["Blazor", "Angular", "Vue", "React", "VanillaJs"];
var selectedItem = Console.Menu("Please Select One beloved frontend framework", true, frontEndFrameworks).Select();

Console.AsciiArt(frontEndFrameworks[selectedItem], GetFrameworkColor(frontEndFrameworks[selectedItem]));

await Task.Delay(2000);
Console.Info("Progress started...", true);
await Task.Delay(1000);
Console.Warning("It seems there is issue in the system", true);
await Task.Delay(1000);
Console.Error("Process failed :(", true);
await Task.Delay(1000);
Console.Info("Retrying...", true);
await Task.Delay(1000);
Console.ReadLine("still working on it");
await Task.Delay(1000);
Console.Success("Progress Succeed", showIcon: true);
await Task.Delay(1000);
Console.WriteLine("Wait it is not completed yet, Check next step", ConsoleColor.Magenta);

Console.WriteLine();
var password = Console.Password("Enter your password:");
Console.Alert($"your password is: {password}", "Password", MessageType.Info);


var result = Console.Confirm("Are you sure to process", true);
if (result)
    Console.Success("Processing", true);
else
    Console.Warning("You cancelled request", true);

Console.ReadKey("Press any key");

string[] plugins = ["TypeScript", "Linter", "Nuxt", "Vite"];
var selectedItems = Console.Checkbox("Select feature that you want to install:", plugins).Select();
for (int i = 0; i < selectedItems.Length; i++)
{
    var plugin = selectedItems[i];
    Console.WriteLine(plugin.Option, (ConsoleColor)i);
}

Console.ReadKey("Press any key" + Environment.NewLine);

// Table
Person[] people2 = [
    new Person("Saeid Doroudi",30, "Tehran"),
    new Person("Saman", 25, "Marand"),
    new Person("Alice", 35, "Zurich"),
    new Person("Alireza", 40, "Tabriz")
];
Console.Table(people2, ConsoleColor.DarkCyan);

Console.ReadKey("Press any key to exit");

ConsoleColor GetFrameworkColor(string framework)
{
    return framework switch
    {
        "Blazor" => ConsoleColor.DarkMagenta,
        "Angular" => ConsoleColor.Red,
        "Vue" => ConsoleColor.Green,
        "React" => ConsoleColor.Blue,
        "JS" => ConsoleColor.Yellow,
        _ => ConsoleColor.White
    };
}



record Person(string Name, int Age, string City);