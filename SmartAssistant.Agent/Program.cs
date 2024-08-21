using SmartAssistant.Agent;
using SmartAssistant.Agent.Plugins;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("initializing configuration...");

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false)
                     .AddUserSecrets<Program>();

builder.Services.AddLogging(logging =>
    {
        var section = builder.Configuration.GetSection("Logging");
        logging.AddConfiguration(section);
    });
builder.Services.AddHttpClient<LightsPlugin>((sp, client) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        client.BaseAddress = new Uri(config["SmartAssistantEndpoint"]!);
    });
 builder.Services.AddHttpClient<DoorSensorsPlugin>((sp, client) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        client.BaseAddress = new Uri(config["SmartAssistantEndpoint"]!);
    });
var host = builder.Build();

Console.WriteLine("initializing smart assistant...");

var credentials = new DefaultAzureCredential();
var kernelBuilder = Kernel.CreateBuilder();
var config = host.Services.GetRequiredService<IConfiguration>();
kernelBuilder.AddAzureOpenAIChatCompletion(
    config["OpenAiModelId"]!,
    endpoint: config["OpenAiEndpoint"]!,
    credentials);

var lightsPlugin = host.Services.GetRequiredService<LightsPlugin>();
kernelBuilder.Plugins.AddFromObject(lightsPlugin);
var doorSensorsPlugin = host.Services.GetRequiredService<DoorSensorsPlugin>();
kernelBuilder.Plugins.AddFromObject(doorSensorsPlugin);

var kernel = kernelBuilder.Build();

OpenAIPromptExecutionSettings executionSettings = new() { 
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions 
};

var history = new ChatHistory();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Welcome to Smart Assistant!");

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"You > ");
    var question = Console.ReadLine()!.Trim();
    if (question.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    history.AddUserMessage(question);

    var result = await chatCompletionService.GetChatMessageContentAsync(
       history,
       executionSettings: openAIPromptExecutionSettings,
       kernel: kernel);

    history.Add(result);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Assistant > " + result);
    Console.WriteLine();
}