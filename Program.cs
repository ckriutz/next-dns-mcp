using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextDnsMcp;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

var apiKey = Environment.GetEnvironmentVariable("NEXTDNS_API_KEY")
    ?? throw new InvalidOperationException("NEXTDNS_API_KEY environment variable is required.");
var profileId = Environment.GetEnvironmentVariable("NEXTDNS_PROFILE_ID")
    ?? throw new InvalidOperationException("NEXTDNS_PROFILE_ID environment variable is required.");

builder.Services.AddSingleton(sp =>
{
    var http = new HttpClient
    {
        BaseAddress = new Uri("https://api.nextdns.io/")
    };
    http.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
    return new NextDnsClient(http, profileId);
});

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "NextDNS MCP Server",
            Version = "1.0.0"
        };
    })
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp();

await app.RunAsync();
