# Lab 03 - Agent Skills

## Purpose

This lab teaches Agent Skills as portable capability packages.

Learners create and consume skills from four source types:

1. File-based skills from `SKILL.md` folders.
2. Code-defined inline skills with `AgentInlineSkill`.
3. Class-based skills with `AgentClassSkill<T>`.
4. MCP/API Center published skills with `UseMcpSkills(...)`.

This lab does not ask an LLM-backed agent to choose the skill. That practical usage belongs in a later lab. Here the focus is packaging, discovery, activation, and safe provider construction.

Reference: https://learn.microsoft.com/en-us/agent-framework/agents/skills?pivots=programming-language-csharp

## Microsoft Agent Framework Components Used

| Skill Type | Component | Lab Example |
|---|---|---|
| File-based | `AgentSkillsProviderBuilder.UseFileSkill(...)` | `skills/training-delivery-readiness`, `skills/student-support-triage` |
| Inline | `AgentInlineSkill` | `training-session-followup-v1` |
| Class-based | `AgentClassSkill<T>` | `student-environment-check-v1` |
| MCP/API Center | `Microsoft.Agents.AI.Mcp`, `UseMcpSkills(...)`, `ModelContextProtocol.Client` | API Center runtime `https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms` |

Package versions:

- `Microsoft.Agents.AI` `1.13.0`
- `Microsoft.Agents.AI.Mcp` `1.13.0-alpha.260703.1`

The MCP package is prerelease because Microsoft marks MCP-based skills as experimental.

## File-Based Skills

The starter pack includes two file-based skills:

```text
src/Lab03SkillDrivenDevelopment/skills/
├── training-delivery-readiness/
│   ├── SKILL.md
│   └── references/readiness-checklist.md
└── student-support-triage/
    ├── SKILL.md
    └── references/support-flow.md
```

These skills are registered with:

```csharp
new AgentSkillsProviderBuilder()
    .UseFileSkill(skillsRoot, options: fileOptions)
```

The lab uses a guarded file script runner that rejects file script execution. This keeps the Day 3 exercise safe while still demonstrating native file-skill registration.

## Inline Skill

The inline skill demonstrates dynamic skill construction in code:

`training-session-followup-v1`

It includes:

- frontmatter
- instructions
- one resource
- one script

Use inline skills when skill content is generated dynamically or needs to close over runtime state such as batch ID.

## Class-Based Skill

The class-based skill demonstrates compiled reusable C# skill logic:

`student-environment-check-v1`

It includes:

- `AgentSkillFrontmatter`
- `[AgentSkillResource]`
- `[AgentSkillScript]`

Use class-based skills when the capability belongs in compiled C# and should be versioned with application code.

## MCP/API Center Published Skills

API Center runtime:

`https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms`

The lab includes a planning manifest:

`src/Lab03SkillDrivenDevelopment/api-center/api-center-published-skills.json`

It lists two published skill examples:

- `pronative-training-operations-skill`
- `pronative-student-environment-skill`

MCP-based skills expect an MCP server that exposes Agent Skills through:

`skill://index.json`

When the API Center runtime is ready to serve MCP skills, enable the live connection:

```powershell
$env:ENABLE_APIC_MCP_SKILLS="true"
$env:APIC_RUNTIME_URL="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
$env:APIC_MCP_ENDPOINT="https://apic-an2607101-fec2ed.data.centralindia.azure-apicenter.ms"
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

If API Center exposes MCP on a route such as `/mcp`, set `APIC_MCP_ENDPOINT` to that exact URL.

By default, the MCP connection is skipped so every student can run the lab without relying on live API Center connectivity.

## Run

```powershell
dotnet run --project .\src\Lab03SkillDrivenDevelopment\Lab03SkillDrivenDevelopment.csproj
```

Expected output:

1. Two file-based skills discovered from the lab folder.
2. Inline skill preview and script result.
3. Class-based skill preview and script result.
4. API Center published skill manifest.
5. Combined provider boundary with `load_skill`, `read_skill_resource`, and `run_skill_script`.

## Trainer Notes

Explain the distinction clearly:

- File-based skills are portable packages.
- Inline skills are runtime-defined packages.
- Class-based skills are compiled C# packages.
- MCP/API Center skills are remotely published packages.

This lab creates and consumes skills. Later labs can attach the same provider to an `AIAgent` so the model chooses when to call `load_skill`, `read_skill_resource`, and `run_skill_script`.
