# ai-native-day01-foundry-csharp

Lean C# companion repo for Day 1: Generative AI Apps.

This repo follows the MS Learn AI-3016 flow, adapted for the ProNative live Azure landing zone.

## MS Learn Exercise Alignment

| Day 1 Topic | MS Learn Exercise | This Repo |
|---|---|---|
| Foundry orientation | `01-Explore-ai-studio` | Uses existing Foundry endpoint/project settings |
| Model catalog and evaluation | `02-model-catalog-evaluation` | Captures model/deployment metadata and evaluation notes |
| Foundry SDK app | `03-foundry-sdk` | C# chat call using live model deployment |
| RAG/context engineering | `04a-use-own-data` | Azure AI Search grounding hook |
| Responsible AI/content filters | `06-Explore-content-filters` | Content-safety reflection and trace capture |

## Student Setup

Required:

- .NET SDK 10
- Azure CLI logged in with the student identity
- access to the student resource group
- access to shared Foundry, Search, and observability resources

Copy `samples/appsettings.sample.json` to `src/Day01FoundryChat/appsettings.json`, then fill your assigned values.

```powershell
cd src/Day01FoundryChat
dotnet run -- --ask "What is AI-native architecture?"
```

## Auth Model

The code supports two training-friendly modes:

1. `BearerToken` for Entra/managed identity token flow.
2. `ApiKey` for fallback trainer-controlled demos where approved.

For production-grade student Azure runtime, use managed identity and do not commit secrets.

