namespace Pronative.MultiAgentTraining.Shared;

// Minimal abstractions that mirror the Microsoft Agent Framework mental model:
// agents have instructions, receive a task, and produce structured messages.
// Replace these with official Agent Framework agent/workflow types in the live implementation.
public interface ITrainingAgent
{
    string Name { get; }

    string Instructions { get; }

    Task<AgentMessage> RunAsync(string task, CancellationToken cancellationToken);
}

public sealed class RoleAgent : ITrainingAgent
{
    private readonly Func<string, AgentMessage> _handler;

    public RoleAgent(string name, string instructions, Func<string, AgentMessage> handler)
    {
        Name = name;
        Instructions = instructions;
        _handler = handler;
    }

    public string Name { get; }

    public string Instructions { get; }

    public Task<AgentMessage> RunAsync(string task, CancellationToken cancellationToken) => Task.FromResult(_handler(task));
}

public static class ConsoleFormatting
{
    public static void Header(string text)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', text.Length));
        Console.WriteLine(text);
        Console.WriteLine(new string('=', text.Length));
    }
}
