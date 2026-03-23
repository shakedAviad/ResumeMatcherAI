using Core.Workflows;
using Frontend.Console;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var provider = Host.CreateApplicationBuilder(args)
    .ConfigureServices()
    .BuildApplication()
    .CreateProvider();

var conversationWorkflow = provider.GetRequiredService<ResumeConversationWorkflow>();

await Utils.WaitForApisAsync();

while (Utils.TryGetUserInput(out var userInput))
{
    Utils.WriteLine(await conversationWorkflow.GetConversationResponse(userInput));
    Utils.Separator();
}
