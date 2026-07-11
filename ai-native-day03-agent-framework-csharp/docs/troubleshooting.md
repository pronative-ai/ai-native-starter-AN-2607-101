# Troubleshooting

## Authentication Error

Run:

```powershell
az login
az account show
```

The lab uses `AzureCliCredential` for local training runs.

## 404 Or Project Not Found

Make sure `AZURE_AI_PROJECT_ENDPOINT` is the project-scoped Foundry endpoint, not the raw Azure OpenAI endpoint.

Expected shape:

```text
https://<resource>.services.ai.azure.com/api/projects/<project-name>
```

## Model Deployment Error

Confirm the deployment exists in the Foundry project:

```powershell
$env:AZURE_OPENAI_CHAT_DEPLOYMENT="gpt-5-mini"
```

## Tool Was Not Called

The lab prompt and agent instructions require `get_batch_readiness_signal`.

If the model still does not call the tool, rerun with the default prompt. The default prompt explicitly asks for the action and observation to come from the tool result.
