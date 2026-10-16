using BasketballMcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

var directory = Path.GetFullPath(Environment.GetEnvironmentVariable("BASKETBALL_DATA_DIR") ?? Path.Combine("data", "synthetic"));
if (args.Contains("--seed-sample"))
{
    await SampleData.WriteAsync(directory);
    Console.WriteLine("Created fictional basketball Parquet files. These are not NBA statistics.");
    return;
}
var store = new BasketballStore(directory);
if (args.Contains("--demo"))
{
    Console.WriteLine(JsonSerializer.Serialize(await store.FreeThrowsAsync(10), new JsonSerializerOptions { WriteIndented = true }));
    return;
}
var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Services.AddSingleton(store);
builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();
await builder.Build().RunAsync();
