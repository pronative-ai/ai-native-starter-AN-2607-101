# Lab 03 - Agent Skills

## Use Case

This lab demonstrates how to create, package, and consume reusable Agent Skills in Microsoft Agent Framework. Skills are modular capabilities that can be discovered, activated, and executed across different agents.

The lab covers three skill types:

- **File-based skills** - SKILL.md folders with frontmatter metadata and reference resources
- **Inline skills** - Programmatically defined with `AgentInlineSkill`
- **Class-based skills** - Compiled C# logic extending `AgentClassSkill<T>`

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- No Azure authentication required (local-first execution)

### Steps

1. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
   ```

2. **Observe the skill discovery and execution output.**

### Optional: API Center/MCP Path

```powershell
$env:ENABLE_APIC_MCP_SKILLS="true"
$env:APIC_RUNTIME_URL="<your-apic-endpoint>"
$env:APIC_MCP_ENDPOINT="<your-mcp-endpoint>"
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| SKILL.md files | File-based skill definitions (included in project) |

## Sample Input

No user input required. The lab automatically discovers and executes skills.

## Expected Output

```
Skill Source 1 - File-Based Skills
==================================
Skills folder: .../skills
- student-support-triage
  Description: Triage student support requests...
  Resources: references/support-flow.md
- training-delivery-readiness
  Description: Check training delivery readiness...
  Resources: references/readiness-checklist.md

Skill Source 2 - Code-Defined Inline Skill
==========================================
Skill:        training-session-followup-v1
Description:  Draft concise follow-up notes...
Inline skill script result:
{
  "batchId": "AN-2607-101",
  "subject": "Day 3 - Agent Skills follow-up",
  "body": "Today we focused on...",
  "nextAction": "Prepare for Lab 04 - State and memory."
}

Skill Source 3 - Class-Based Skill
==================================
Skill:        student-environment-check-v1
Description:  Check whether a learner device is ready...
Class skill script result:
{
  "studentId": "ST-2606-1000",
  "status": "needs_attention",
  "requiredActions": ["Run az login..."]
}
```

## Key Learning Points

1. **Skill lifecycle** - Discovery → Activation → Execution → Reuse
2. **File-based skills** - Best for portable, team-owned skill packages
3. **Inline skills** - Good for dynamic, runtime-generated skill content
4. **Class-based skills** - Best when skill logic belongs in compiled C#
5. **Provider boundary** - This lab creates/consumes skills; a later lab wires the provider into an agent

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Skills folder not found | Check `AppContext.BaseDirectory/skills` directory |
| Script execution fails | Ensure skill has proper `[AgentSkillScript]` attribute |

## Reference

- [Agent Framework Skills](https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp)
