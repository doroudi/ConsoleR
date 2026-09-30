
using ConsoleR;
using ConsoleR.Chat.Models;
using ConsoleR.Loading;
using System.Drawing;
using Console = ConsoleR.Console;

// ---------------------------------------------------------------------------------------------
// ConsoleR sample application
// The sections are ordered by how quickly they show
// what the library does: the chat and the menus come first, the details of the output later.
// ---------------------------------------------------------------------------------------------

var sectionCount = 0;

Section("CHAT", "AI chat with the input box sticky at the bottom of the window", ShowChat, pause: false);
Section("MENU", "Scrollable menus, one choice at a time", ShowMenu);
Section("CHECKBOX", "Select several options at once", ShowCheckbox);
Section("TABLE", "A table from any list of objects", ShowTable);
Section("COLORS", "Colors by name, by RGB value or as an inline tag", ShowColors);
Section("MESSAGES", "Info, success, warning and error output", ShowMessages);
Section("ALERT", "A message in a box of its own", ShowAlerts);
Section("INPUTS", "Password and confirmation", ShowInputs);
Section("BOX", "Read a line inside a box", ShowBoxInput);
await SectionAsync("SPINNER", "Wait for work that takes a while", ShowSpinner);
Section("COW", "A cow that says something about the output", ShowCowSay);
Section("ART", "ASCII art: outline, filled and shadow", ShowAsciiArt, pause: false);

Closing();

// ---------------------------------------------------------------------------------------------
// The features of the library
// ---------------------------------------------------------------------------------------------

void ShowChat()
{
    // The history fills the window from the top, the input box sticks to the bottom.
    // The bot is a local echo here, any model or service can be plugged in through the reply.
    // Enter sends the message, Shift+Enter starts a new line, Escape leaves the chat.
    var chat = Console.Chat(
        message => ResponseToChat(message),
        new ConsoleChatSettings
        {
            InputTitle = "You",
            MaxInputLines = 5,
            ThinkingText = "Thinking...",
            UserStyle = new ChatMessageStyle { Label = "You", Foreground = ConsoleColor.Black, Background = ConsoleColor.Cyan },
            BotStyle = new ChatMessageStyle { Label = "ConsoleR Bot", Foreground = ConsoleColor.White, Background = ConsoleColor.DarkGreen }
        });

    chat.AddMessage(ConsoleChatRole.Bot, "Hi! Ask me anything. Shift+Enter starts a new line, the arrows scroll the history, Escape leaves the chat.");
    chat.Run();
}

async IAsyncEnumerable<string> ResponseToChat(string message)
{
    var reply = $"The bot is a local echo here, any model or service can be plugged in through the reply. The response is streamed to the chat as it is generated. {message}";

    var words = reply.Split(' ');
    foreach (var word in words)
    {
        await Task.Delay(100);
        yield return word + " ";
    }
}

void ShowMenu()
{
    // A long list: only the options that fit into the window are shown, the box scrolls with the selection.
    string[] languages = ["C#", "Java", "Python", "JavaScript", "TypeScript", "Go", "Rust", "C++", "Kotlin", "Ruby",
        "Swift", "PHP", "Dart", "Elixir", "Scala", "Haskell", "F#", "Clojure", "Lua", "Perl", "R", "Julia", "Zig", "Nim"];

    var selectedLanguage = Console.Menu("Select your favorite programming language:", languages).Select();
    Console.Info($"You selected {languages[selectedLanguage]}", showIcon: true);

    // A box that shows five options at a time, the numbers keep the position inside the whole list.
    string[] frontEndFrameworks = ["Blazor", "Angular", "Vue", "React", "Svelte", "Solid", "Qwik", "Astro", "VanillaJs"];

    var selectedFramework = Console.Menu("Select one frontend framework:", 5, frontEndFrameworks).Select();
    Console.AsciiArt(frontEndFrameworks[selectedFramework], AsciiArtStyle.Outline, GetFrameworkColor(frontEndFrameworks[selectedFramework]));
}

void ShowTable()
{
    Person[] people =
    [
        new Person("Doroudi", 37, "Marand", "Lorem Ipsum is simply dummy text of the printing and typesetting industry"),
        new Person("Raimar", 40, "Dresden", "Lorem Ipsum is simply dummy text of the printing and typesetting industry"),
        new Person("Alice", 35, "Zurich", "Lorem Ipsum is simply dummy text of the printing and typesetting industry"),
        new Person("Andishe", 45, "Dubai", "Lorem Ipsum is simply dummy text of the printing and typesetting industry")
    ];

    // Every property of the objects becomes a column.
    Console.Table(people.Select(person => new { person.Name, person.City }));

    // Columns can be left out and long cells can be shortened.
    Console.WriteLine();
    Console.Table(people, ConsoleColor.DarkCyan, ignoredColumns: [nameof(Person.Age)], maxColumnLength: 25);
}

void ShowColors()
{
    // Inline tags: [foreground] or [foreground:background]
    Console.WriteLine("[red]Hello World![/]");
    Console.WriteLine("[white:magenta]Hello World![/]");
    Console.WriteLine("Hello World! This is [blue]formatted[/] text, you can [black:yellow]highlight[/] your important message easily!");
    Console.WriteLine();

    // A color by name, by RGB value or by hex code
    var outputText = "Welcome to ConsoleR! A set of utilities that makes console applications look good.";
    Console.Write("Hello ", "#FFCC00");
    Console.Write("World", ConsoleColor.Green);
    Console.Write(";\n", Color.BlueViolet);
    Console.WriteLine(outputText, Color.Magenta);
    Console.WriteLine(outputText, "#00CCFF");
    Console.WriteLine(outputText, ConsoleColor.DarkGreen);
}

void ShowMessages()
{
    // The four levels of a message, with or without an icon.
    Console.Info("Progress started");
    Console.Success("Progress succeeded", showIcon: true);
    Console.Warning("It seems there is an issue in the system", showIcon: true);
    Console.Error("Process failed :(", showIcon: true);
    Console.WriteLine("Wait, it is not completed yet", ConsoleColor.Magenta);
    Console.WriteBool(true, "The flag is set", "The flag is not set");
}

void ShowAlerts()
{
    Console.Alert($"Welcome to the ConsoleR sample application {Environment.NewLine}This is a tour through the features of the library", " ConsoleR ", MessageType.Info);
    Console.Alert("This is a warning", " Warning ", MessageType.Warning);
    Console.Alert("An error message goes here", " ERROR ", MessageType.Error);
    Console.Alert("It can be a successful message also :)", string.Empty, MessageType.Success);
}

void ShowCheckbox()
{
    string[] plugins = ["TypeScript", "Linter", "Nuxt", "Vite"];
    var selectedItems = Console.Checkbox("Select the features you want to install:", plugins).Select();

    Console.WriteLine();
    ConsoleColor[] palette = [ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta];
    for (var i = 0; i < selectedItems.Length; i++)
        Console.WriteLine($" - {selectedItems[i].Option}", palette[i % palette.Length]);
}

void ShowInputs()
{
    var password = Console.Password("Enter your password:");
    Console.Success($"Your password has {password.Length} characters", showIcon: true);

    Console.WriteLine();
    var confirmed = Console.Confirm("Are you sure to process", true);
    if (confirmed) Console.Success("Processing", showIcon: true);
    else Console.Warning("You cancelled the request", showIcon: true);
}

void ShowBoxInput()
{
    var content = Console.ReadInBox("You:");
    Console.Info($"Bot: {content}");

    content = Console.ReadInBox("You:", ConsoleColor.Yellow);
    Console.Info($"Bot: {content}");
}

async Task ShowSpinner()
{
    // A spinner for work that takes a while. A failure is reported by the spinner itself.
    var failing = new Spinner();
    await failing.Start(() =>
    {
        Thread.Sleep(1000);
        throw new Exception("Something went wrong");
    }, "Starting app");

    // The text of a spinner that is already running can be changed.
    var spinner = new Spinner();
    await spinner.Start(() =>
    {
        var step = 0;
        while (step < 10)
        {
            step++;
            Thread.Sleep(100);
        }
        spinner.SetText("Almost done...");
    }, "Restart app");
}

void ShowCowSay()
{
    Console.CowSay("Hello! This is the [red:gray]CowSay[/] feature");
    Console.CowSay("Hello\nThis is [black:magenta]multiline[/] [green]output![/]\nDoes it look good?", "××");
}

void ShowAsciiArt()
{
    // The same word in every style of the library, the sections above use the shadow style.
    Console.WriteLine("Outline", ConsoleColor.DarkGray);
    Console.AsciiArt("ART", AsciiArtStyle.Outline, ConsoleColor.Gray);

    Console.WriteLine("Filled", ConsoleColor.DarkGray);
    Console.AsciiArt("ART", AsciiArtStyle.Filled, ConsoleColor.Yellow);

    Console.WriteLine("Shadow", ConsoleColor.DarkGray);
    Console.AsciiArt("ART", AsciiArtStyle.Shadow, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
}

void Closing()
{
    Console.ClearAll();
    Console.AsciiArt("ConsoleR", AsciiArtStyle.Shadow, ConsoleColor.Green, ConsoleColor.DarkGreen);
    Console.Success("That is all, thanks for trying ConsoleR!", showIcon: true);
    Console.WriteLine();
    Console.ReadKey("Press any key to exit");
}

// ---------------------------------------------------------------------------------------------
// The helpers of the sample application
// ---------------------------------------------------------------------------------------------

// Clears the screen, draws the banner of the section and runs the feature.
void Section(string title, string subtitle, Action feature, bool pause = true)
{
    SectionBanner(title, subtitle);
    feature();
    if (pause) Pause();
}

// The same for a feature that has to be awaited, for example the spinner.
async Task SectionAsync(string title, string subtitle, Func<Task> feature, bool pause = true)
{
    SectionBanner(title, subtitle);
    await feature();
    if (pause) Pause();
}

void SectionBanner(string title, string subtitle)
{
    Console.ClearAll();
    Console.AsciiArt(title, AsciiArtStyle.Shadow, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
    Console.WriteLine($" {++sectionCount}. {subtitle}", ConsoleColor.DarkGray);
    Console.WriteLine();
}

void Pause()
{
    Console.WriteLine();
    Console.ReadLine("Press Enter to continue...", color: ConsoleColor.DarkGray);
}

ConsoleColor GetFrameworkColor(string framework) => framework switch
{
    "Blazor" => ConsoleColor.DarkMagenta,
    "Angular" => ConsoleColor.Red,
    "Vue" => ConsoleColor.Green,
    "React" => ConsoleColor.Blue,
    "Svelte" => ConsoleColor.DarkRed,
    "Solid" => ConsoleColor.DarkBlue,
    _ => ConsoleColor.White
};

record Person(string Name, int Age, string City, string Description);
