using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

const string CYAN  = "\u001b[36m";
const string GREEN = "\u001b[32m";
const string YELLOW = "\u001b[33m";
const string DIM   = "\u001b[2m";
const string BOLD  = "\u001b[1m";
const string RESET = "\u001b[0m";

Console.WriteLine();
Console.WriteLine($"{BOLD}{CYAN}╔══════════════════════════════════════════╗{RESET}");
Console.WriteLine($"{BOLD}{CYAN}║       Foundry Local  -  Chat Client      ║{RESET}");
Console.WriteLine($"{BOLD}{CYAN}╚══════════════════════════════════════════╝{RESET}");
Console.WriteLine();
Console.WriteLine($"{DIM}Initializing Foundry Local...{RESET}");

const string WEB_PORT = "11434";

await FoundryLocalManager.CreateAsync(
    new Configuration
    {
        AppName = "foundry-local-demo",
        Web = new Configuration.WebService
        {
            Urls = $"http://127.0.0.1:{WEB_PORT}"
        }
    },
    NullLogger.Instance);

var catalog = await FoundryLocalManager.Instance.GetCatalogAsync();
var model = await catalog.GetModelAsync("qwen2.5-0.5b")
    ?? throw new Exception("Model not found.");

Console.WriteLine($"{DIM}Downloading model...{RESET}");
await model.DownloadAsync();
Console.WriteLine($"{DIM}Loading model...{RESET}");
await model.LoadAsync();

await FoundryLocalManager.Instance.StartWebServiceAsync();

Console.WriteLine();
Console.WriteLine($"{GREEN}{BOLD}Model loaded: {model.Id}{RESET}");
Console.WriteLine();
Console.WriteLine($"{BOLD}{CYAN}╔══════════════════════════════════════════════════════════════╗{RESET}");
Console.WriteLine($"{BOLD}{CYAN}║  Foundry Local Web Service - OpenAI-Compatible Endpoint      ║{RESET}");
Console.WriteLine($"{BOLD}{CYAN}╠══════════════════════════════════════════════════════════════╣{RESET}");
Console.WriteLine($"{BOLD}{CYAN}║{RESET}  Base URL   : {GREEN}http://127.0.0.1:{WEB_PORT}{RESET}                         {BOLD}{CYAN}║{RESET}");
Console.WriteLine($"{BOLD}{CYAN}║{RESET}  Chat       : {GREEN}http://127.0.0.1:{WEB_PORT}/v1/chat/completions{RESET}     {BOLD}{CYAN}║{RESET}");
Console.WriteLine($"{BOLD}{CYAN}║{RESET}  Models     : {GREEN}http://127.0.0.1:{WEB_PORT}/v1/models{RESET}               {BOLD}{CYAN}║{RESET}");
Console.WriteLine($"{BOLD}{CYAN}║{RESET}  Model Name : {YELLOW}{model.Id}{RESET}  {BOLD}{CYAN}║{RESET}");
Console.WriteLine($"{BOLD}{CYAN}╚══════════════════════════════════════════════════════════════╝{RESET}");
Console.WriteLine();
Console.WriteLine($"{DIM}Any application can now connect to this endpoint using OpenAI-compatible API.{RESET}");
Console.WriteLine($"{DIM}Example: curl http://127.0.0.1:{WEB_PORT}/v1/chat/completions -H \"Content-Type: application/json\" -d '{{\"model\":\"qwen2.5-0.5b\",\"messages\":[{{\"role\":\"user\",\"content\":\"hello\"}}]}}'{RESET}");
Console.WriteLine();
Console.WriteLine($"{DIM}Type your message and press Enter. Type 'exit' to quit.{RESET}");
Console.WriteLine();
Console.WriteLine($"{DIM}────────────────────────────────────────────{RESET}");

var messages = new List<ChatMessage>();

while (true)
{
    Console.Write($"{YELLOW}{BOLD}You ▸ {RESET}");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine();
        Console.WriteLine($"{DIM}Stopping web service...{RESET}");
        await FoundryLocalManager.Instance.StopWebServiceAsync();
        Console.WriteLine($"{DIM}Unloading model...{RESET}");
        await model.UnloadAsync();
        Console.WriteLine($"{DIM}Goodbye!{RESET}");
        break;
    }

    messages.Add(new ChatMessage { Role = "user", Content = input });

    Console.Write($"{CYAN}{BOLD}AI  ▸ {RESET}");

    var response = await (await model.GetChatClientAsync()).CompleteChatAsync(messages.ToArray());
    var content = response.Choices![0].Message.Content ?? "(empty)";

    messages.Add(new ChatMessage { Role = "assistant", Content = content });

    Console.WriteLine($"{CYAN}{content}{RESET}");
    Console.WriteLine($"{DIM}────────────────────────────────────────────{RESET}");
}
