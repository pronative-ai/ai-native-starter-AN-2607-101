namespace Pronative.Day05.Shared;

public static class ConsoleTable
{
    public static void Header(string title)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine(new string('=', title.Length));
    }

    public static void Row(string name, string value)
    {
        Console.WriteLine($"{name,-28} {value}");
    }

    public static void Warning(string message)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = original;
    }

    public static void Success(string message)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = original;
    }

    public static void ApplicationStart(int labNumber)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine($"                     Application {labNumber:D2} Start");
        Console.WriteLine("================================================================================");
    }

    public static void ApplicationEnd(int labNumber)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine($"                     Application {labNumber:D2} End");
        Console.WriteLine("================================================================================");
    }
}
