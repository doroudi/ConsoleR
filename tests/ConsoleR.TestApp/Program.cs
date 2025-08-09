using ConsoleR;
using ConsoleR.Loading;
using Console = ConsoleR.Console;


var spinner = new Spinner();
await spinner.Start(() =>
{
    int x = 0;
    while (x < 10)
    {
        x++;
        Thread.Sleep(100);
    }
}, "Starting app");

Console.AsciiArt("ConsoleR", ConsoleColor.Yellow);
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
Console.Alert($"your password is: {password}", "Password", ConsoleMessageType.Info);


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

Console.ReadKey("Press any key");

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