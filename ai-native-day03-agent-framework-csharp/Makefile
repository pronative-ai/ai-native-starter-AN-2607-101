SHELL = cmd.exe

LAB01 = src\Lab01AgenticReasoningLoop
LAB02 = src\Lab02FlowEngineering
LAB03 = src\Lab03SkillDrivenDevelopment
LAB04 = src\Lab04ConversationsMemory
LAB05 = src\Lab05HarnessEngineering
LAB06 = src\Lab06HybridRagWorkflow
LAB07 = src\Lab07WorkflowAgent

ALL_LABS = $(LAB01) $(LAB02) $(LAB03) $(LAB04) $(LAB05) $(LAB06) $(LAB07)

.PHONY: all restore build clean lab1 lab2 lab3 lab4 lab5 lab6 lab7

all: build

restore:
	dotnet restore $(LAB01)
	dotnet restore $(LAB02)
	dotnet restore $(LAB03)
	dotnet restore $(LAB04)
	dotnet restore $(LAB05)
	dotnet restore $(LAB06)
	dotnet restore $(LAB07)

build: restore
	dotnet build $(LAB01) --no-restore
	dotnet build $(LAB02) --no-restore
	dotnet build $(LAB03) --no-restore
	dotnet build $(LAB04) --no-restore
	dotnet build $(LAB05) --no-restore
	dotnet build $(LAB06) --no-restore
	dotnet build $(LAB07) --no-restore

clean:
	dotnet clean $(LAB01)
	dotnet clean $(LAB02)
	dotnet clean $(LAB03)
	dotnet clean $(LAB04)
	dotnet clean $(LAB05)
	dotnet clean $(LAB06)
	dotnet clean $(LAB07)

lab1:
	dotnet run --project $(LAB01)

lab2:
	dotnet run --project $(LAB02)

lab3:
	dotnet run --project $(LAB03)

lab4:
	dotnet run --project $(LAB04)

lab5:
	dotnet run --project $(LAB05)

lab6:
	dotnet run --project $(LAB06)

lab7:
	dotnet run --project $(LAB07)
