namespace ConsoleR;

public partial class Console {
    public static bool Confirm(string message)
    {
        return new ConsoleConfirm(message).GetAnswer();
    }
    public static bool Confirm(string message, bool defaultValue) {
        return new ConsoleConfirm(message, defaultValue).GetAnswer();
    }
}

internal class ConsoleConfirm(string Message) {
    private readonly bool? _defaultValue;

    internal ConsoleConfirm(string Message, bool defaultValue): this(Message) {
        
        _defaultValue = defaultValue;
    }
    internal bool GetAnswer() {
        var confirmActions = "y/n";
        if(_defaultValue.HasValue)
        {
            confirmActions = _defaultValue.Value? "Y/n" : "y/N";
        }
        Console.Write($"{Message} [{confirmActions}]: ");
        var result =  Console.ReadKey();
        System.Console.WriteLine();
        if(result.Key == ConsoleKey.Enter)
        {
            if(_defaultValue.HasValue)
                return _defaultValue.Value;
            else {
                Console.Error("you should press y or n keys");            
                return GetAnswer();
            }
        }
        
        return result.Key == ConsoleKey.Y;
    }
}