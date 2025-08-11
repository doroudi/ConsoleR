namespace ConsoleR.Loading;

public class Spinner
{
    int spinStep = 0;
    string _message = "";
    private CancellationTokenSource _cancellationSource;
    private Task? _task;
    static string[] _pattern = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];
    static string[] _legacyPattern = ["-","\\","|","/"];
    private static string[] Pattern => ConsoleHelpers.IsLegacy ? _legacyPattern : _pattern;

    public Spinner()
    {
        _cancellationSource = new CancellationTokenSource();
    }
    private void Spin()
    {
        System.Console.SetCursorPosition(0, 0);
        Console.Clear();
        Console.Write($"{Pattern[spinStep++]} {_message}");
        spinStep %= Pattern.Length;

        System.Console.SetCursorPosition(System.Console.CursorLeft - _message.Length - 1, System.Console.CursorTop);
    }

    public async Task Start(Action action, string message = "")
    {
        await Start(() => Task.Run(action), message);
    }

    public async Task Start(Func<Task> asyncAction, string message = "")
    {
        _message = message;
        System.Console.CursorVisible = false;
        System.Console.WriteLine("\x1b]9;4;3;100\x07"); //Set Windows Terminal to loading state
        _task = Task.Run(async () =>
        {
            while (!_cancellationSource.IsCancellationRequested)
            {
                Spin();
                await Task.Delay(120).ConfigureAwait(false);
            }
        });

        try
        {
            // Await the async action
            await asyncAction.Invoke().ConfigureAwait(false);
            Stop(_message);
        }
        catch (Exception ex)
        {
            Stop(errorMessage: ex.Message);
        }
        finally
        {
            System.Console.WriteLine("\x1b]9;4;0;100\x07"); //Reset loading state of windows terminal
            System.Console.CursorVisible = true;
        }
    }

    public void Stop(string? message = null, string? errorMessage = null)
    {
        if (_cancellationSource.IsCancellationRequested)
            return;

        System.Console.SetCursorPosition(0, 0);
        Console.Clear();
        if (string.IsNullOrEmpty(errorMessage))
        {
            Console.Success(message ?? _message, true);
        }
        else
        {
            Console.Error(errorMessage, true);
        }

        _cancellationSource.Cancel();
        _task?.Wait();
    }

    public void SetText(string message)
    {
        _message = message;
    }
}
