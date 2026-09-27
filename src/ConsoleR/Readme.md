# ConsoleR

ConsoleR is set of utilities to make awesome console apps in .Net

## Features

- Colorful output
- Menu
- CheckBox
- AsciiArt
- WriteLine
- Password
- Alert
- Table
- Chat

### How To Use

```csharp
using System.Text;
using ConsoleR;
using ConsoleR.Chat.Models;
using ConsoleR.Loading;
using System.Drawing;
using Console = ConsoleR.Console;

var outputText = "Welcome to ConsoleR!";

Console.AsciiArt("Output Color", ConsoleColor.Yellow);
// The outline can be filled or get a shadow that falls to the bottom right
Console.AsciiArt("Output Color", AsciiArtStyle.Filled, ConsoleColor.Yellow);
Console.AsciiArt("Output Color", AsciiArtStyle.Shadow, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
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
Console.Menu("Select your favorite programming language:", "C#", "Java", "Python", "JavaScript", "Go", "Rust", "C++", "Kotlin", "Ruby", "Swift", "PHP", "Dart", "Elixir", "Scala", "Haskell").Select();


var content = Console.ReadInBox("You:");
Console.Info($"Bot: {content}");
content = Console.ReadInBox("You:", ConsoleColor.Yellow);

// Chat: the history fills the window and the input box sticks to the bottom.
// The box grows with the message, the spinner of the library runs while the bot is thinking.
var chat = Console.Chat(
    message => AskTheModel(message),           // or: async message => await AskTheModelAsync(message)
    new ChatSettings
    {
        InputTitle = "You",
        MaxInputLines = 5,                     // how far the box grows, null grows it as far as the window allows
        ThinkingText = "Thinking...",
        UserStyle = new ChatMessageStyle { Label = "You", Foreground = ConsoleColor.Black, Background = ConsoleColor.Cyan },
        BotStyle = new ChatMessageStyle { Label = "ConsoleR Bot", Foreground = ConsoleColor.White, Background = ConsoleColor.DarkBlue }
    });

chat.AddMessage(ChatRole.Bot, "Hi! Ask me anything, Shift+Enter starts a new line, Escape leaves the chat.");
chat.Run();

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
// Menus that do not fit into the window scroll inside a box, a fixed view box is one argument away.
var selectedItem = Console.Menu("Please Select One beloved frontend framework", 5, frontEndFrameworks).Select();

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
    new Person("Saeid Doroudi",30, "Dresden", "Lorem Ipsum is simply dummy text of the printing and typesetting industry"),
    new Person("Saman", 25, "Marand", "Lorem Ipsum is simply dummy text of the printing and typesetting industry"),
    new Person("Alice", 35, "Zurich", "Lorem Ipsum is simply dummy text of the printing and typesetting industry"),
    new Person("Alireza", 40, "Tabriz", "Lorem Ipsum is simply dummy text of the printing and typesetting industry")
];
Console.Table(people2, ConsoleColor.DarkCyan, ignoredColumns: [nameof(Person.Age)], maxColumnLength: 25);

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



record Person(string Name, int Age, string City, string Description);
```

### Chat

`Enter` sends the message, `Shift+Enter` (or `Ctrl+Enter`) starts a new line and `Escape` leaves the
chat. The input box grows with the message up to `MaxInputLines`, longer lines wrap inside it. While
the answer of the bot is on the way the spinner of the library runs in the input box.

The arrow keys scroll the history, `PageUp` and `PageDown` jump through it, `Home` and `End` move the
caret to the beginning and the end of the line it is on, the newest message is always at the bottom.

An answer that arrives in pieces can grow in the chat, the reply then returns `null` to say that it
drew its own message:

```csharp
ConsoleChat? chat = null;
chat = Console.Chat(async message =>
{
    var answer = new StringBuilder();
    chat!.AddMessage(ChatRole.Bot);

    await foreach (var chunk in StreamTheModel(message))
    {
        answer.Append(chunk);
        chat.UpdateLastMessage(answer.ToString());
    }

    return null;
});

chat.Run();
```

### Output

![ConsoleR](https://raw.githubusercontent.com/doroudi/ConsoleR/refs/heads/main/docs/ConsoleRBanner.png)
