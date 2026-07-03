.PHONY: build run clean restore watch config

PROJECT := src/Day01FoundryChat/Day01FoundryChat.csproj
PROMPT ?= "Explain AI-native architecture for enterprise fullstack engineering."

build:
	dotnet build $(PROJECT)

run:
	dotnet run --project $(PROJECT) -- --ask $(PROMPT)

clean:
	dotnet clean $(PROJECT)
	rm -rf src/Day01FoundryChat/bin src/Day01FoundryChat/obj

restore:
	dotnet restore $(PROJECT)

watch:
	dotnet watch --project $(PROJECT) run -- --ask $(PROMPT)

config:
	cp samples/appsettings.sample.json src/Day01FoundryChat/appsettings.json
	@echo "Config copied. Edit src/Day01FoundryChat/appsettings.json with your credentials."