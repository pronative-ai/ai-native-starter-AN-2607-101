using System.Text.Json;
using Microsoft.Agents.AI;
using Pronative.Day03.Shared;

var config = Day03TrainingConfig.Load(args);

Day03Console.PrintAppStart();
Day03Console.PrintHeader(config, "Lab 03 - Agent Skills");

Console.WriteLine("This lab creates and consumes Agent Skills from three source types:");
Console.WriteLine("- file-based skills from SKILL.md folders");
Console.WriteLine("- code-defined skills with AgentInlineSkill");
Console.WriteLine("- class-based skills with AgentClassSkill<T>");
Console.WriteLine();

Console.WriteLine("Important boundary");
Console.WriteLine("==================");
Console.WriteLine("This lab is about skill packaging, discovery, activation, and direct consumption.");
Console.WriteLine("A later lab will attach this provider to an agent and let the model choose skills during an agent run.");
Console.WriteLine();

var skillsRoot = Path.Combine(AppContext.BaseDirectory, "skills");

Console.WriteLine("Skill Source 1 - File-Based Skills");
Console.WriteLine("==================================");
Console.WriteLine($"Skills folder: {skillsRoot}");
Console.WriteLine("Native registration: AgentSkillsProviderBuilder.UseFileSkill(skillsRoot)");
Console.WriteLine();

// File-based skills are registered from SKILL.md folders with frontmatter metadata and reference resources
var fileSkillSummaries = FileSkillReader.ReadSkillSummaries(skillsRoot);
foreach (var skill in fileSkillSummaries)
{
    Console.WriteLine($"- {skill.Name}");
    Console.WriteLine($"  Description: {skill.Description}");
    Console.WriteLine($"  Resources:   {string.Join(", ", skill.Resources)}");
}

Console.WriteLine();
Console.WriteLine("Skill Source 2 - Code-Defined Inline Skill");
Console.WriteLine("==========================================");
// Inline skills are built programmatically with AgentInlineSkill, adding resources and scripts at runtime
var inlineSkill = CreateSessionFollowUpInlineSkill(config);
await PrintCodeSkillPreviewAsync(inlineSkill);
var followUpScript = await inlineSkill.GetScriptAsync("draft-followup", CancellationToken.None)
    ?? throw new InvalidOperationException("Inline skill script 'draft-followup' was not registered.");
var followUp = await followUpScript.RunAsync(
    inlineSkill,
    JsonSerializer.SerializeToElement(new
    {
        sessionTitle = "Day 3 - Agent Skills",
        learnerSignal = "Learners can explain file, inline, class, and MCP skill sources.",
        nextLab = "Lab 04 - State and memory"
    }),
    EmptyServiceProvider.Instance,
    CancellationToken.None);
Console.WriteLine("Inline skill script result:");
Console.WriteLine(ToJson(followUp));
Console.WriteLine();

Console.WriteLine("Skill Source 3 - Class-Based Skill");
Console.WriteLine("==================================");
// Class-based skills extend AgentClassSkill<T> with compiled C# logic, resources as properties, and scripts as methods
var classSkill = new StudentEnvironmentClassSkill();
await PrintCodeSkillPreviewAsync(classSkill);
var environmentScript = await classSkill.GetScriptAsync("check-student-environment", CancellationToken.None)
    ?? throw new InvalidOperationException("Class skill script 'check-student-environment' was not registered.");
var environmentResult = await environmentScript.RunAsync(
    classSkill,
    JsonSerializer.SerializeToElement(new
    {
        studentId = config.StudentId,
        dotnetSdkVersion = Environment.Version.ToString(),
        azureCliLoggedIn = false,
        foundryToolkitInstalled = true,
        repoCloned = true
    }),
    EmptyServiceProvider.Instance,
    CancellationToken.None);
Console.WriteLine("Class skill script result:");
Console.WriteLine(ToJson(environmentResult));
Console.WriteLine();



Day03Console.PrintLabStart(3);
Console.WriteLine("Trainer Checkpoint");
Console.WriteLine("==================");
Console.WriteLine("1. File-based skills are best for portable team-owned skill packages.");
Console.WriteLine("2. Inline skills are good for dynamic runtime-generated skill content.");
Console.WriteLine("3. Class-based skills are good when skill logic belongs in compiled C#.");
Console.WriteLine("4. This lab creates and consumes skills; a later lab wires the provider into an agent.");
Day03Console.PrintLabEnd(3);

Day03Console.PrintAppEnd();

// Inline skill: defines frontmatter, instructions, template resource, and a C# script function for draft follow-ups
static AgentInlineSkill CreateSessionFollowUpInlineSkill(Day03TrainingConfig config)
{
    var frontmatter = new AgentSkillFrontmatter(
        "training-session-followup-v1",
        "Draft concise follow-up notes after a ProNative training session.",
        "ProNative Day 3 v1; code-defined inline skill")
    {
        License = "ProNative training",
        AllowedTools = AgentSkillsProvider.ReadSkillResourceToolName
    };

    return new AgentInlineSkill(
            frontmatter,
            instructions:
                """
                Use this skill after a practical training session.
                Read or generate session context, then produce a short follow-up note.
                Keep the message practical: what was built, what evidence to keep, and what comes next.
                """,
            serializerOptions: null,
            argumentMarshaler: null)
        .AddResource(
            "followup-template",
            """
            Subject: {sessionTitle} follow-up

            Include:
            - What learners practiced
            - Evidence learners should retain
            - The next lab or readiness action
            """,
            "Reusable follow-up structure.")
        .AddScript(
            "draft-followup",
            DraftFollowUp,
            "Drafts a concise learner follow-up note.",
            serializerOptions: null);

    FollowUpDraft DraftFollowUp(string sessionTitle, string learnerSignal, string nextLab) =>
        new(
            BatchId: config.BatchId,
            Subject: $"{sessionTitle} follow-up",
            Body:
                $"Today we focused on {sessionTitle}. Evidence signal: {learnerSignal} " +
                $"Please keep your lab output and notes. Next: {nextLab}.",
            NextAction: $"Prepare for {nextLab}.");
}

static async Task PrintCodeSkillPreviewAsync(AgentSkill skill)
{
    var content = await skill.GetContentAsync(CancellationToken.None);
    Console.WriteLine($"Skill:        {skill.Frontmatter.Name}");
    Console.WriteLine($"Description:  {skill.Frontmatter.Description}");
    Console.WriteLine($"Preview:      {Preview(content)}");
    Console.WriteLine();

    static string Preview(string content)
    {
        var normalized = content.ReplaceLineEndings(" ");
        return normalized.Length <= 180 ? normalized : normalized[..180] + "...";
    }
}

static string ToJson(object? value) =>
    JsonSerializer.Serialize(value, new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });

// Class-based skill: frontmatter via override, resources via [AgentSkillResource] properties, scripts via [AgentSkillScript] methods
internal sealed class StudentEnvironmentClassSkill : AgentClassSkill<StudentEnvironmentClassSkill>
{
    public override AgentSkillFrontmatter Frontmatter { get; } = new(
        "student-environment-check-v1",
        "Check whether a learner device is ready for C# Agent Framework labs.",
        "ProNative Day 3 v1; class-based C# skill")
    {
        License = "ProNative training",
        AllowedTools = AgentSkillsProvider.ReadSkillResourceToolName
    };

    protected override string Instructions =>
        """
        Use this skill when a learner cannot run a lab or before a practical begins.
        Report missing prerequisites as concrete actions. Do not guess that Azure access works.
        """;

    [AgentSkillResource("required-tools")]
    public string RequiredTools =>
        """
        Required local tools:
        - .NET SDK 10
        - VS Code
        - Foundry Toolkit extension
        - Azure CLI
        - Git

        Required access:
        - Student repository
        - Foundry project visibility
        - Training resource-group role assignments
        """;

    [AgentSkillScript("check-student-environment")]
    private static StudentEnvironmentAssessment CheckStudentEnvironment(
        string studentId,
        string dotnetSdkVersion,
        bool azureCliLoggedIn,
        bool foundryToolkitInstalled,
        bool repoCloned)
    {
        var actions = new List<string>();
        if (!dotnetSdkVersion.StartsWith("10.", StringComparison.OrdinalIgnoreCase))
        {
            actions.Add("Install or select .NET SDK 10.");
        }

        if (!azureCliLoggedIn)
        {
            actions.Add("Run az login with the correct training identity.");
        }

        if (!foundryToolkitInstalled)
        {
            actions.Add("Install the Foundry Toolkit extension in VS Code.");
        }

        if (!repoCloned)
        {
            actions.Add("Clone or copy the assigned starter repository.");
        }

        return new StudentEnvironmentAssessment(
            StudentId: studentId,
            DotnetSdkVersion: dotnetSdkVersion,
            Status: actions.Count == 0 ? "ready" : "needs_attention",
            RequiredActions: actions,
            RuntimeNote: "This class-based skill can later be attached to an agent through the combined provider.");
    }
}

// Reads file-based skills from SKILL.md directories, parses YAML-like frontmatter, and discovers reference resources
internal static class FileSkillReader
{
    public static IReadOnlyList<FileSkillSummary> ReadSkillSummaries(string skillsRoot)
    {
        if (!Directory.Exists(skillsRoot))
        {
            return [];
        }

        return Directory.GetDirectories(skillsRoot)
            .Select(ReadSkillSummary)
            .OrderBy(summary => summary.Name, StringComparer.Ordinal)
            .ToArray();
    }

    private static FileSkillSummary ReadSkillSummary(string skillDirectory)
    {
        var skillFile = Path.Combine(skillDirectory, "SKILL.md");
        var skillMarkdown = File.ReadAllText(skillFile);
        var frontmatter = ParseFrontmatter(skillMarkdown);
        var resources = Directory.Exists(Path.Combine(skillDirectory, "references"))
            ? Directory.GetFiles(Path.Combine(skillDirectory, "references"), "*.*", SearchOption.AllDirectories)
                .Select(path => Path.GetRelativePath(skillDirectory, path).Replace('\\', '/'))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray()
            : [];

        return new FileSkillSummary(
            Name: frontmatter.GetValueOrDefault("name", Path.GetFileName(skillDirectory)),
            Description: frontmatter.GetValueOrDefault("description", "(no description)"),
            Resources: resources);
    }

    private static Dictionary<string, string> ParseFrontmatter(string skillMarkdown)
    {
        using var reader = new StringReader(skillMarkdown);
        if (reader.ReadLine() is not "---")
        {
            return [];
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? line;
        while ((line = reader.ReadLine()) is not null && line is not "---")
        {
            var separator = line.IndexOf(':');
            if (separator <= 0)
            {
                continue;
            }

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim().Trim('"');
            values[key] = value;
        }

        return values;
    }
}

internal sealed class EmptyServiceProvider : IServiceProvider
{
    public static readonly EmptyServiceProvider Instance = new();

    private EmptyServiceProvider()
    {
    }

    public object? GetService(Type serviceType) => null;
}

internal sealed record FileSkillSummary(
    string Name,
    string Description,
    IReadOnlyList<string> Resources);

internal sealed record StudentEnvironmentAssessment(
    string StudentId,
    string DotnetSdkVersion,
    string Status,
    IReadOnlyList<string> RequiredActions,
    string RuntimeNote);

internal sealed record FollowUpDraft(
    string BatchId,
    string Subject,
    string Body,
    string NextAction);


