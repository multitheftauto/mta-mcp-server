using MtaDebugCompanionMcp.Common;
using MtaDebugCompanionMcp.Common.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.AddMtaDebugCompanionMcpServices(builder.Configuration.GetValue<string>("serverHost"), builder.Configuration.GetValue<string>("apiKey"));

builder.Services
    .AddMcpServer()
    .WithToolsFromAssembly(typeof(MtaDebugCompanionTools).Assembly)
    .WithHttpTransport(x =>
    {
        x.Stateless = true;
    });

var app = builder.Build();

app.MapMcp("/");
app.Run();
