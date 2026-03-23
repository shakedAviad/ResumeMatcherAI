using Backend.API;

await WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .BuildApplication()
    .RunAsync();