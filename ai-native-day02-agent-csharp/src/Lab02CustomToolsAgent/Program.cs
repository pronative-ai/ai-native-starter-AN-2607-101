using System.Text.Json;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Pronative.AgentTraining.Shared;

var config = TrainingConfig.Load(args);
TrainingConfigConsole.Print(config, "Lab 02 - Custom Functions as Agent Tools");

Console.WriteLine("MS Learn mapping:");
Console.WriteLine("- Python lab creates astronomy-agent with function tools.");
Console.WriteLine("- This C# v2 lab uses Microsoft Agent Framework so .NET functions are registered as real agent tools.");
Console.WriteLine("- The model chooses the tool; the framework executes the C# function and returns the result.");
Console.WriteLine();

Console.Write("Ask astronomy-agent, or press Enter for a sample request: ");
var prompt = Console.ReadLine();

if (string.IsNullOrWhiteSpace(prompt))
{
    prompt = """
    I am in South America. Find the next visible astronomy event, calculate the cost for 5 hours on a premium telescope with normal priority,
    and prepare a short observation report for Bellows College.
    """;
}

string answer;
if (config.UseLiveFoundry)
{
    // Agent Framework uses the Foundry project endpoint, not the /openai/v1 endpoint.
    // Authentication follows the Microsoft Learn C# pattern: run `az login` and use AzureCliCredential.
    var projectClient = new AIProjectClient(new Uri(config.ProjectEndpoint), new AzureCliCredential());

    // Each AIFunctionFactory.Create call turns a normal .NET method into a tool schema
    // the model can inspect and choose at runtime. The method is not called by our code directly;
    // it is called by the framework only when the model emits the matching tool call.
    var nextVisibleEventTool = AIFunctionFactory.Create(
        (Func<string, string>)AstronomyTools.NextVisibleEvent,
        name: "next_visible_event",
        description: "Get the next visible astronomical event for a location such as south_america, australia, or india.");

    var calculateCostTool = AIFunctionFactory.Create(
        (Func<string, double, string, string>)AstronomyTools.CalculateObservationCost,
        name: "calculate_observation_cost",
        description: "Calculate telescope rental cost from telescope tier, observation hours, and priority.");

    var generateReportTool = AIFunctionFactory.Create(
        (Func<string, string, string, double, string, string, string>)AstronomyTools.GenerateObservationReport,
        name: "generate_observation_report",
        description: "Generate a short text observation report after event and cost details are known.");

    var agent = projectClient.AsAIAgent(
        model: config.ModelDeployment,
        name: config.AgentName,
        description: "Day 2 C# astronomy assistant with .NET function tools",
        instructions: """
        You are an astronomy observations assistant.
        Help users find astronomical events, calculate telescope rental costs, and prepare observation reports.
        Use tools when the user asks for event visibility, cost calculation, or report generation.
        Do not invent tool results. If a tool is needed, call it.
        Explain the final answer clearly after tool results are available.
        """,
        tools: [nextVisibleEventTool, calculateCostTool, generateReportTool]);

    var run = await agent.RunAsync(prompt);
    answer = run.ToString();
}
else
{
    answer = """
    Live Foundry execution is disabled.

    Set UseLiveFoundry=true after the trainer verifies:
    - az login is completed
    - ProjectEndpoint points to the Foundry project endpoint
    - ModelDeployment is available
    - the trainer/student identity has permission to call the Foundry project

    In live mode this lab does not fake function calling. Microsoft Agent Framework exposes the C# methods as tools,
    the model selects the tool, and the framework invokes the .NET method.
    """;
}

TrainingConfigConsole.PrintLlmResponseHeader();
Console.WriteLine(answer);
TrainingConfigConsole.PrintLlmResponseFooter();

internal static class AstronomyTools
{
    public static string NextVisibleEvent(string location)
    {
        var normalized = Normalize(location);
        var result = normalized switch
        {
            "south_america" => new
            {
                event_name = "Jupiter-Venus Conjunction",
                event_type = "planetary_conjunction",
                date = "May 1",
                visible_from = new[] { "south_america" },
                viewing_tip = "Look west shortly after sunset with a clear horizon."
            },
            "australia" => new
            {
                event_name = "Perseid Meteor Shower",
                event_type = "meteor_shower",
                date = "August 12",
                visible_from = new[] { "australia" },
                viewing_tip = "Best after midnight from a dark-sky location."
            },
            "india" => new
            {
                event_name = "Partial Lunar Eclipse",
                event_type = "eclipse",
                date = "September 7",
                visible_from = new[] { "india" },
                viewing_tip = "Visible without telescope; binoculars improve detail."
            },
            _ => new
            {
                event_name = "Lunar Eclipse",
                event_type = "eclipse",
                date = "September 7",
                visible_from = new[] { normalized },
                viewing_tip = "Check local weather and light pollution before observing."
            }
        };

        return JsonSerializer.Serialize(result);
    }

    public static string CalculateObservationCost(string telescopeTier, double hours, string priority)
    {
        var baseRate = telescopeTier.ToLowerInvariant() switch
        {
            "standard" => 125,
            "advanced" => 250,
            "premium" => 375,
            _ => 125
        };

        var multiplier = priority.ToLowerInvariant() switch
        {
            "high" => 1.25,
            "normal" => 1.0,
            "low" => 0.85,
            _ => 1.0
        };

        return JsonSerializer.Serialize(new
        {
            telescope_tier = telescopeTier,
            hours,
            priority,
            cost = baseRate * hours * multiplier,
            currency = "USD"
        });
    }

    public static string GenerateObservationReport(
        string eventName,
        string location,
        string telescopeTier,
        double hours,
        string priority,
        string observerName)
    {
        // This lab intentionally returns report text instead of writing a file.
        // In real enterprise tools, this boundary is where approval, storage,
        // audit logging, and policy checks belong.
        var report = $"""
        Observation Report
        Observer: {observerName}
        Event: {eventName}
        Location: {location}
        Telescope tier: {telescopeTier}
        Duration: {hours} hours
        Priority: {priority}
        """;

        return JsonSerializer.Serialize(new
        {
            report,
            message = $"Generated observation report for {observerName}."
        });
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant().Replace(" ", "_").Replace("-", "_");
    }
}
