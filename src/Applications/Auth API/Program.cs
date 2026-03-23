
using Auth.API;

await WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .BuildApplication()
    .RunAsync();