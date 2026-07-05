LABS := Lab01FoundryAgentClient Lab02CustomToolsAgent Lab03McpAgentClient Lab04FoundryIqAgentClient Lab06WorkflowClient Lab07AgentFrameworkSingleAgent Lab08AgentFrameworkMultiAgent
PROJECTS := $(addprefix src/,$(LABS)) src/Pronative.AgentTraining.Shared

.PHONY: all build clean restore rebuild test $(PROJECTS) lab1 lab2 lab3 lab4 lab6 lab7 lab8

all: restore build

$(PROJECTS):
	dotnet build $@/$(@F).csproj --no-restore

build: $(PROJECTS)

restore:
	dotnet restore src/Lab01FoundryAgentClient
	dotnet restore src/Lab02CustomToolsAgent
	dotnet restore src/Lab03McpAgentClient
	dotnet restore src/Lab04FoundryIqAgentClient
	dotnet restore src/Lab06WorkflowClient
	dotnet restore src/Lab07AgentFrameworkSingleAgent
	dotnet restore src/Lab08AgentFrameworkMultiAgent
	dotnet restore src/Pronative.AgentTraining.Shared

clean:
	dotnet clean src/Lab01FoundryAgentClient
	dotnet clean src/Lab02CustomToolsAgent
	dotnet clean src/Lab03McpAgentClient
	dotnet clean src/Lab04FoundryIqAgentClient
	dotnet clean src/Lab06WorkflowClient
	dotnet clean src/Lab07AgentFrameworkSingleAgent
	dotnet clean src/Lab08AgentFrameworkMultiAgent
	dotnet clean src/Pronative.AgentTraining.Shared

rebuild: clean restore build

test:
	dotnet test src/Lab01FoundryAgentClient
	dotnet test src/Lab02CustomToolsAgent
	dotnet test src/Lab03McpAgentClient
	dotnet test src/Lab04FoundryIqAgentClient
	dotnet test src/Lab06WorkflowClient
	dotnet test src/Lab07AgentFrameworkSingleAgent
	dotnet test src/Lab08AgentFrameworkMultiAgent
	dotnet test src/Pronative.AgentTraining.Shared

lab1: 
	dotnet restore src/Lab01FoundryAgentClient
	dotnet build src/Lab01FoundryAgentClient
	cd src/Lab01FoundryAgentClient & dotnet run

lab2:
	dotnet restore src/Lab02CustomToolsAgent
	dotnet build src/Lab02CustomToolsAgent
	cd src/Lab02CustomToolsAgent & dotnet run

lab3:
	dotnet restore src/Lab03McpAgentClient
	dotnet build src/Lab03McpAgentClient
	cd src/Lab03McpAgentClient & dotnet run

lab4:
	dotnet restore src/Lab04FoundryIqAgentClient
	dotnet build src/Lab04FoundryIqAgentClient
	cd src/Lab04FoundryIqAgentClient & dotnet run

lab6:
	dotnet restore src/Lab06WorkflowClient
	dotnet build src/Lab06WorkflowClient
	cd src/Lab06WorkflowClient & dotnet run

lab7:
	dotnet restore src/Lab07AgentFrameworkSingleAgent
	dotnet build src/Lab07AgentFrameworkSingleAgent
	cd src/Lab07AgentFrameworkSingleAgent & dotnet run

lab8:
	dotnet restore src/Lab08AgentFrameworkMultiAgent
	dotnet build src/Lab08AgentFrameworkMultiAgent
	cd src/Lab08AgentFrameworkMultiAgent & dotnet run

token:
	az account get-access-token --resource https://ai.azure.com --query accessToken -o tsv
 