# Lab 06 - Retrieval-Grounded RAG for Agentic Workflow

## Use Case

This lab demonstrates Retrieval-Augmented Generation (RAG) using Microsoft Agent Framework Workflows with Cosmos DB for structured data retrieval. The workflow orchestrates:

1. **Query planning** - Normalizes the user question for retrieval
2. **Cosmos DB retrieval** - Fetches training knowledge records
3. **Foundry-backed answer agent** - Generates grounded response with citations
4. **Grounding verification** - Checks if answer cites retrieved documents
5. **Bounded retry** - Expands query and retries if grounding fails
6. **Typed result** - Produces structured output with evidence

## How to Execute

### Prerequisites

- .NET 10.0 SDK installed
- Azure CLI installed and authenticated (`az login`)
- Azure AI Foundry project with GPT-5-mini deployment
- Cosmos DB account with training knowledge data

### Steps

1. **Authenticate with Azure:**
   ```powershell
   az login
   ```

2. **Set environment variables** (see [Root README](../../README.md#required-foundry-settings) for values):
   ```powershell
   $env:AZURE_AI_PROJECT_ENDPOINT="<your-project-endpoint>"
   $env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
   $env:COSMOS_ENDPOINT="<your-cosmos-endpoint>"
   $env:COSMOS_DATABASE="db-an2607101-training"
   $env:COSMOS_CONTAINER="training-knowledge"
   ```

3. **Optional: Seed Cosmos DB with sample data:**
   ```powershell
   $env:COSMOS_CREATE_IF_NOT_EXISTS="true"
   $env:COSMOS_SEED_SAMPLE_DATA="true"
   dotnet run --project .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj -- --seed
   ```

4. **Run the lab:**
   ```powershell
   dotnet run --project .\src\Lab06HybridRagWorkflow\Lab06HybridRagWorkflow.csproj
   ```

5. **Enter a grounded question** when prompted, or press Enter for the default.

## Resources Required

| Resource | Purpose |
|----------|---------|
| .NET 10.0 SDK | Build and run the application |
| Azure CLI | Authentication |
| Azure AI Foundry Project | Hosts the answer agent |
| GPT-5-mini deployment | Powers the answer generation |
| Cosmos DB | Training knowledge records for retrieval |

## Sample Input

**Default question (press Enter):**
```
For Day 3 and Day 4 AI-Native training, what evidence exists about retrieval choices,
workflow agents, skills, gateway control, and how Cosmos DB differs from Azure AI Search and HorizonDB?
```

**Negative grounding test:**
```
What exact GPU model and hourly price are used by this batch for HorizonDB?
```

## Expected Output

```
[workflow:event] executor_completed=PlanRetrieval
[Retrieval plan JSON]

[workflow:event] executor_completed=AnswerWithFoundryAgent
[Grounded answer with citations]

[workflow:event] executor_completed=VerifyGrounding
[Grounding verification: passes/fails]

Final Retrieval-Grounded RAG Result
===================================
{
  "terminalStatus": "grounded",
  "question": "...",
  "answer": "Based on the retrieved records...\n\nAnswer\n[Citations]\n[doc:pn-d3-cosmos-grounding]",
  "retrievedDocuments": [...],
  "citedDocumentIds": ["pn-d3-cosmos-grounding"],
  "verificationFindings": ["Retrieved 3 record(s)...", "Answer cited 1 retrieved record(s)..."],
  "attempts": 1,
  "nextAction": "Use the cited answer as the grounded response..."
}

Evidence artifact: .../artifacts/ST-2606-1000/day03-lab06-retrieval-grounded-rag-result.json
```

## Key Learning Points

1. **RAG workflow** - Explicit orchestration of retrieval → answer → verification → retry
2. **Cosmos DB retrieval** - Structured/semi-structured operational grounding
3. **Citation verification** - Answer must cite `[doc:<id>]` from retrieved records
4. **Grounding check** - System verifies answer uses retrieved evidence
5. **Bounded retry** - Expands query if first attempt fails verification
6. **Retrieval comparison** - Cosmos DB vs Azure AI Search vs HorizonDB (conceptual)

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Cosmos returns no data | Check `COSMOS_DATABASE`, `COSMOS_CONTAINER`; seed sample data |
| Answer not grounded | Verify Cosmos has relevant training knowledge records |
| Authentication error | Run `az account show` to verify login |
| Retrieval fails | Check `COSMOS_ENDPOINT` and partition key (`/batchId`) |

## Reference

- [Cosmos DB .NET SDK](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3)
