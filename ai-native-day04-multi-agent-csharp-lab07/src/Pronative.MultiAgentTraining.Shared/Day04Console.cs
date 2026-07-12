namespace Pronative.MultiAgentTraining.Shared;

public static class Day04Console
{
    public static void PrintAppStart()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================================================");
        Console.WriteLine("                          Application Started                           ");
        Console.WriteLine("========================================================================");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintAppEnd()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("========================================================================");
        Console.WriteLine("                         Application Completed                          ");
        Console.WriteLine("========================================================================");
        Console.ResetColor();
    }

    public static void PrintLabStart(int labNumber)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"================== Lab {labNumber} Result Start =================");
        Console.ResetColor();
    }

    public static void PrintLabEnd(int labNumber)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"================== Lab {labNumber} Result End ====================");
        Console.ResetColor();
    }
}
