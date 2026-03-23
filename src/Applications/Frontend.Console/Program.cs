using Core.Interfaces;
using Core.Tools;
using Core.Workflows;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Frontend.Console;
using Infrastructure.AI.Agents;
using Infrastructure.AI.Prompts;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAI;
using System.Reflection;


var time = DateTime.Now.AddMinutes(1);

while (DateTime.Now < time)
{
    
    await Task.Delay(TimeSpan.FromSeconds(10));
}


var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<SecretsManager>(optional: true);

builder.Services.AddSingleton<SecretsManager>();

builder.Services.AddTransient<IChatClient>(sp =>
{
    var openAiOptions = sp.GetRequiredService<SecretsManager>();
    var client = new OpenAIClient(openAiOptions.OpenAiApiKey);
    var chatClient = client.GetChatClient(openAiOptions.ChatModel);

    return chatClient.AsIChatClient();
}); 

builder.Services.AddSingleton<IResumeRoutingAgent>(sp =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();

    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
    {
        Name = "ResumeRoutingAgent",
        ChatOptions = new()
        {
            Instructions = ResumeRoutingPrompt.Instructions
        }
    });
    
    return new OpenAiResumeRoutingAgent(agent);
});

builder.Services.AddScoped<IResumeConversationAgent>(sp =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    
    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
    {
        Name = "ResumeConversationAgent",
        ChatOptions = new()
        {
            Instructions = ResumeConversationPrompt.Instructions,                       
        },
        
    });
   
    return new OpenAiResumeConversationAgent(agent);
});

builder.Services.AddScoped<ResumeConversationWorkflow>(sp => 
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(sp.GetRequiredService<SecretsManager>().BaseBackendAPIURL);

    return new ResumeConversationWorkflow(
        sp.GetRequiredService<IResumeConversationAgent>(),
        sp.GetRequiredService<IResumeRoutingAgent>(),
        httpClient        
    );
});


var serviceProvider = builder.Services.BuildServiceProvider();
var conversationWorkflow = serviceProvider.GetRequiredService<ResumeConversationWorkflow>();

Console.WriteLine("Resume Assistant is ready.");
Console.WriteLine("Type 'exit' to quit.");

while (true)
{
    Console.Write("> ");
    var userInput = Console.ReadLine();

    if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(userInput))
    {
        continue;
    }

    var reply = await conversationWorkflow.GetConversationResponse(userInput);
    Console.WriteLine(reply);
}


