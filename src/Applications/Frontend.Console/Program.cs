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

