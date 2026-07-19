# Lab05FoundryLocalCpu vs foundry-local — Concept Comparison

Both projects use **local Foundry inference** (`Microsoft.AI.Foundry.Local` SDK + ONNX Runtime on CPU). The difference is in **scope and purpose**, not in the inference approach.

## Comparison Table

| | **Lab05FoundryLocalCpu** | **foundry-local** |
|---|---|---|
| **Purpose** | Formal training lab (Day 5 curriculum) | Informal standalone demo |
| **Part of solution?** | Yes (`make lab5`) | No |
| **Inference mode** | Streaming (`CompleteChatStreamingAsync`) | Non-streaming (`CompleteChatAsync`) |
| **Lifecycle coverage** | Full: discover EPs → download → load → infer → unload | Partial: download → load → infer only |
| **EP handling** | Yes — `DiscoverEps()`, `DownloadAndRegisterEpsAsync()` | None |
| **SDK variant** | `WinML` (Windows-only, net8.0-windows) | Standard (cross-platform, net10.0) |
| **Telemetry/tracing** | Full — `ActivitySource`, operational JSON events | None |
| **Config** | Env vars + `.env` file | Hardcoded |
| **Interaction** | Single one-shot prompt | Multi-turn REPL loop |
| **Error handling** | Structured try/catch with trainer fallback | None |
| **Lines of code** | ~218 | ~67 |

## Summary

- **foundry-local** — A stripped-down interactive chat demo showing basic SDK usage. Quick developer reference for the Foundry Local SDK.
- **Lab05FoundryLocalCpu** — The instrumented, curriculum-driven version that teaches the **complete local model lifecycle** with operational observability (telemetry, structured events, error resilience). Both run on local CPU inference.
