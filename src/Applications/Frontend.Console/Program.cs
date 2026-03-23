using Core.Interfaces;
using Core.Tools;
using Core.Workflows;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using Domain.Models;
using Frontend.Console;
using Frontend.Console.Auth;
using Frontend.Console.Auth.Handlers;
using Frontend.Console.Auth.Interfaces;
using Infrastructure.AI.Agents;
using Infrastructure.AI.Prompts;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using System.Reflection;


var time = DateTime.Now.AddMinutes(1);

while (DateTime.Now < time)
{    
    await Task.Delay(TimeSpan.FromSeconds(10));
}




//var builder = Host.CreateApplicationBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

//builder.Logging.SetMinimumLevel(LogLevel.Warning);

//builder.Configuration.AddUserSecrets<SecretsManager>(optional: true);

//builder.Services.AddSingleton<SecretsManager>();

//builder.Services.AddTransient<JwtAuthorizationHandler>();

//builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
//{
//    client.BaseAddress = new Uri(SharedConfiguration.BaseBackendAuthAPIURL);
//});



//builder.Services.AddTransient<IChatClient>(sp =>
//{
//    var openAiOptions = sp.GetRequiredService<SecretsManager>();
//    var client = new OpenAIClient(openAiOptions.OpenAiApiKey);
//    var chatClient = client.GetChatClient(openAiOptions.ChatModel);

//    return chatClient.AsIChatClient();
//}); 

//builder.Services.AddSingleton<IResumeRoutingAgent>(sp =>
//{
//    var chatClient = sp.GetRequiredService<IChatClient>();

//    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
//    {
//        Name = "ResumeRoutingAgent",
//        ChatOptions = new()
//        {
//            Instructions = ResumeRoutingPrompt.Instructions
//        }
//    });

//    return new OpenAiResumeRoutingAgent(agent);
//});

//builder.Services.AddScoped<IResumeConversationAgent>(sp =>
//{
//    var chatClient = sp.GetRequiredService<IChatClient>();

//    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
//    {
//        Name = "ResumeConversationAgent",
//        ChatOptions = new()
//        {
//            Instructions = ResumeConversationPrompt.Instructions,                       
//        },

//    });

//    return new OpenAiResumeConversationAgent(agent);
//});

//builder.Services.AddHttpClient<ResumeConversationWorkflow>((sp, client) => 
//{
//    client.BaseAddress = new Uri(sp.GetRequiredService<SecretsManager>().BaseBackendAPIURL);
//}).AddHttpMessageHandler<JwtAuthorizationHandler>();


//var serviceProvider = builder.Services.BuildServiceProvider();
//var conversationWorkflow = serviceProvider.GetRequiredService<ResumeConversationWorkflow>();


var provider = Host.CreateApplicationBuilder(args)
    .ConfigureServices()
    .BuildApplication()
    .CreateProvider();

var conversationWorkflow = provider.GetRequiredService<ResumeConversationWorkflow>();


await WaitForApisAsync();

while (true)
{
    if(Utils.TryGetUserInput(out var userInput)) 
    {
        Utils.WriteLine(await conversationWorkflow.GetConversationResponse(userInput));
    }
    else
    {
        break;
    }

    Utils.Separator();
}

async Task WaitForApisAsync()
{
    Console.WriteLine("Waiting for all APIs to start...");
    var reportInterval = TimeSpan.FromSeconds(1);
    

    for (var remaining = TimeSpan.FromSeconds(10); remaining > TimeSpan.Zero; remaining -= reportInterval) 
    {
        Console.Write($"\rWaiting for APIs... Remaining: {remaining:mm\\:ss}   ");

        await Task.Delay(reportInterval);
    }

    Console.Write("\rWaiting for APIs... Remaining: 00:00   ");
    Console.WriteLine();
    Console.WriteLine("All waiting time completed.");
    Console.WriteLine();
    Console.WriteLine("Resume Assistant is ready.");
    Console.WriteLine();
    Console.WriteLine("Type 'exit' to quit.");
    Utils.Separator();
}

