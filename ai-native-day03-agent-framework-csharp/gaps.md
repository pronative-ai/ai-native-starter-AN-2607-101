# Gaps Report: Missing `.env` Configuration & Dotenv Setup

## Overview

Across all 7 labs + 1 shared library, **18 `Environment.GetEnvironmentVariable()` calls** exist across **5 source files**, yet:

- **No `.env` file** exists anywhere in the project tree
- **No `DotNetEnv` (or similar) NuGet package** is referenced in any `.csproj`
- **No dotenv-loading logic** exists in any `Program.cs`
- All environment variables must be set manually via OS environment, terminal session, or launch profile — no documented automated fallback

---

## Affected Projects

### 1. `src/Pronative.Day03.Shared/Day03TrainingConfig.cs` (Shared Library)

| Line | Variable | Fallback |
|------|----------|----------|
| 29 | `BATCH_ID` | `BatchId` property default |
| 30 | `STUDENT_ID` | `StudentId` property default |
| 31 | `AZURE_AI_PROJECT_ENDPOINT` | `PROJECT_ENDPOINT`, then `ProjectEndpoint` default |
| 34 | `AZURE_OPENAI_CHAT_DEPLOYMENT` | `ModelDeployment` default |
| 35 | `FOUNDRY_AGENT_NAME` | `AgentName` default |

This is the central config class. **6 env var reads**, no dotenv loading. The `Load()` method reads `appsettings.json` via `JsonSerializer.Deserialize`, then overlays env vars — but never loads `.env`.

---

### 2. `src/Lab03SkillDrivenDevelopment/Program.cs`

| Line | Variable | Fallback |
|------|----------|----------|
| 407 | `APIC_RUNTIME_URL` | Hardcoded URL |
| 410 | `APIC_MCP_ENDPOINT` | `runtimeUrl` value |
| 414 | `ENABLE_APIC_MCP_SKILLS` | Checked for `"true"` |

**3 env var reads**, no `.env` loading at all.

---

### 3. `src/Lab05HarnessEngineering/Program.cs`

| Line | Variable | Fallback |
|------|----------|----------|
| 216 | `AZURE_OPENAI_ENDPOINT` | `AZURE_AI_FOUNDRY_OPENAI_ENDPOINT`, then hardcoded URL |
| 217 | `AZURE_AI_FOUNDRY_OPENAI_ENDPOINT` | See above |

**2 env var reads**, no `.env` loading.

---

### 4. `src/Lab06HybridRagWorkflow/Program.cs`

| Line | Variable | Fallback |
|------|----------|----------|
| 509 | `AZURE_SEARCH_ENDPOINT` | Hardcoded URL |
| 511 | `AZURE_SEARCH_INDEX_NAME` | Auto-generated `idx-{studentSlug}-rag` |
| 513 | `AZURE_SEARCH_ADMIN_KEY` | **`null` (no fallback)** |
| 514 | `HYBRID_RAG_RETRIEVAL_MODE` | `"azure-ai-search-text"` |
| 516 | `HYBRID_RAG_TOP` | `3` |

**5 env var reads**, no `.env` loading. Notably, `AZURE_SEARCH_ADMIN_KEY` has **no fallback** — if the env var is not set, it will be `null`, which will likely cause a runtime failure.

---

### 5. `src/Lab07WorkflowAgent/Program.cs`

| Line | Variable | Fallback |
|------|----------|----------|
| 322 | `AZURE_OPENAI_ENDPOINT` | `AZURE_AI_FOUNDRY_OPENAI_ENDPOINT`, then hardcoded URL |
| 323 | `AZURE_AI_FOUNDRY_OPENAI_ENDPOINT` | See above |

**2 env var reads**, no `.env` loading.

---

## Complete List of All 16 Distinct Environment Variables

| # | Variable | Location | Has Fallback? |
|---|----------|----------|:---:|
| 1 | `APIC_RUNTIME_URL` | Lab03 | Yes |
| 2 | `APIC_MCP_ENDPOINT` | Lab03 | Yes |
| 3 | `AZURE_AI_FOUNDRY_OPENAI_ENDPOINT` | Lab05, Lab07 | Yes |
| 4 | `AZURE_AI_PROJECT_ENDPOINT` | Shared | Yes |
| 5 | `AZURE_OPENAI_CHAT_DEPLOYMENT` | Shared | Yes |
| 6 | `AZURE_OPENAI_ENDPOINT` | Lab05, Lab07 | Yes |
| 7 | `AZURE_SEARCH_ADMIN_KEY` | Lab06 | **No** |
| 8 | `AZURE_SEARCH_ENDPOINT` | Lab06 | Yes |
| 9 | `AZURE_SEARCH_INDEX_NAME` | Lab06 | Yes |
| 10 | `BATCH_ID` | Shared | Yes |
| 11 | `ENABLE_APIC_MCP_SKILLS` | Lab03 | Yes |
| 12 | `FOUNDRY_AGENT_NAME` | Shared | Yes |
| 13 | `HYBRID_RAG_RETRIEVAL_MODE` | Lab06 | Yes |
| 14 | `HYBRID_RAG_TOP` | Lab06 | Yes |
| 15 | `PROJECT_ENDPOINT` | Shared | Yes |
| 16 | `STUDENT_ID` | Shared | Yes |

---

## Key Gaps Identified

1. **No dotenv library** (`DotNetEnv` or similar) referenced in any `.csproj` across the entire solution.
2. **No `.env` file** present at any level (root or project).
3. **No code loads `.env`** — not in `Program.cs`, not in the config classes, not in the shared library.
4. **`AZURE_SEARCH_ADMIN_KEY`** has **no fallback default** — it will silently be `null` if unset, likely causing a cryptic runtime error.
5. **Inconsistent configuration approach** — some labs use `appsettings.json` (via the shared library's `Day03TrainingConfig.Load()`), others read env vars directly in `Program.cs`, and none use .NET's `IConfiguration` builder pattern that could unify JSON + env + `.env` sources.
6. **No documentation** in `README.md` or any lab doc explaining what environment variables need to be set or how to configure them via `.env`.

---

## Recommended Fix

1. Add `DotNetEnv` NuGet package to each project (or to `Directory.Build.props` for centralized management).
2. Create a root `.env` file (or per-project `.env` files) with all variable placeholders (excluding secrets).
3. Add `.env` to `.gitignore` (if not already).
4. Load `.env` at startup in each `Program.cs` (or centrally in the shared library's config loader) via `DotNetEnv.Env.Load()`.
5. Alternatively (more idiomatic for .NET): adopt `IConfiguration` with `AddJsonFile("appsettings.json")` + `AddEnvironmentVariables()` + `AddDotNetEnv()`, moving all env var reads into strongly-typed config POCOs.
