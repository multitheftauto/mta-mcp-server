using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MtaWikiMcp.Common;
using MtaWikiMcp.Common.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddMtaWikiMcpServices();

builder.Services
    .AddMcpServer()
    .WithToolsFromAssembly(typeof(MtaWikiTools).Assembly)
    .WithStdioServerTransport();

var app = builder.Build();

await app.RunAsync();
