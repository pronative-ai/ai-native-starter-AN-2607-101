.PHONY: up build lab1 lab2 lab3 lab4 lab5 lab6 lab7 lab8 foundry-local clean gateway-up gateway-down gateway-logs gateway-ps

up: gateway-up

build:
	dotnet build ai-native-day05-ops-governance-csharp.slnx

lab1:
	dotnet run --project src/Lab01ObservabilityTelemetry/Lab01ObservabilityTelemetry.csproj

lab2:
	dotnet run --project src/Lab02FoundryOperationalTrace/Lab02FoundryOperationalTrace.csproj

lab3:
	dotnet run --project src/Lab03AgentGatewayOperationalClient/Lab03AgentGatewayOperationalClient.csproj

lab4:
	dotnet run --project src/Lab04AiFinOpsEvidence/Lab04AiFinOpsEvidence.csproj

lab5:
	dotnet run --project src/Lab05FoundryLocalCpu/Lab05FoundryLocalCpu.csproj

lab6:
	dotnet run --project src/Lab06RunpodNeocloudClient/Lab06RunpodNeocloudClient.csproj

lab7:
	dotnet run --project src/Lab07GovernancePolicyCheck/Lab07GovernancePolicyCheck.csproj

lab8:
	dotnet run --project src/Lab08MafMcpAgent/Lab08MafMcpAgent.csproj

foundry-local:
	dotnet run --project src/foundry-local/foundry-local.csproj

clean:
	dotnet clean ai-native-day05-ops-governance-csharp.slnx

gateway-up:
	docker compose -f agentgateway/docker-compose.yaml up -d

gateway-down:
	docker compose -f agentgateway/docker-compose.yaml down

gateway-logs:
	docker compose -f agentgateway/docker-compose.yaml logs -f

gateway-ps:
	docker compose -f agentgateway/docker-compose.yaml ps
