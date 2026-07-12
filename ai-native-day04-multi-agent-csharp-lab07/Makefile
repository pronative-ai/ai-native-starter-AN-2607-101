SHELL = cmd.exe

LAB1 = src\Lab01MultiAgentArchitecture
LAB2 = src\Lab02CoordinatorWorkerAgents
LAB3_EXPOSURE = src\Lab03A2AAgentExposure
LAB3_CONSUMER = src\Lab03A2AAgentConsumer
LAB4 = src\Lab04AgentUserInteractionBoundary
LAB5 = src\Lab05ProtocolToolBoundary
LAB6 = src\Lab06AgentGatewayClient
LAB7 = src\Lab07GatewayObservabilityControl

ALL_LABS = $(LAB1) $(LAB2) $(LAB3_EXPOSURE) $(LAB3_CONSUMER) $(LAB4) $(LAB5) $(LAB6) $(LAB7)

.PHONY: all restore build clean lab1 lab2 lab3 lab3-exposure lab3-consumer lab4 lab5 lab6 lab7

all: build

restore:
	dotnet restore $(LAB1)
	dotnet restore $(LAB2)
	dotnet restore $(LAB3_EXPOSURE)
	dotnet restore $(LAB3_CONSUMER)
	dotnet restore $(LAB4)
	dotnet restore $(LAB5)
	dotnet restore $(LAB6)
	dotnet restore $(LAB7)

build: restore
	dotnet build $(LAB1) --no-restore
	dotnet build $(LAB2) --no-restore
	dotnet build $(LAB3_EXPOSURE) --no-restore
	dotnet build $(LAB3_CONSUMER) --no-restore
	dotnet build $(LAB4) --no-restore
	dotnet build $(LAB5) --no-restore
	dotnet build $(LAB6) --no-restore
	dotnet build $(LAB7) --no-restore

clean:
	dotnet clean $(LAB1)
	dotnet clean $(LAB2)
	dotnet clean $(LAB3_EXPOSURE)
	dotnet clean $(LAB3_CONSUMER)
	dotnet clean $(LAB4)
	dotnet clean $(LAB5)
	dotnet clean $(LAB6)
	dotnet clean $(LAB7)

lab1:
	dotnet run --project $(LAB1)

lab2:
	dotnet run --project $(LAB2)

lab3:
	cmd /c start "Lab03 - A2A Agent Exposure" cmd /c "dotnet run --project $(LAB3_EXPOSURE) && pause"
	timeout /t 5 /nobreak >nul
	dotnet run --project $(LAB3_CONSUMER)

lab3-exposure:
	dotnet run --project $(LAB3_EXPOSURE)

lab3-consumer:
	dotnet run --project $(LAB3_CONSUMER)

lab4:
	dotnet run --project $(LAB4)

lab5:
	dotnet run --project $(LAB5)

lab6:
	dotnet run --project $(LAB6)

lab7:
	dotnet run --project $(LAB7)
