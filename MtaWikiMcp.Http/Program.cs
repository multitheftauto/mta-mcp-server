using MtaWikiMcp.Common;
using MtaWikiMcp.Common.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMtaWikiMcpServices();

builder.Services
    .AddMcpServer()
    .WithToolsFromAssembly(typeof(MtaWikiTools).Assembly)
    .WithHttpTransport(x =>
    {
        x.Stateless = true;
    });

var app = builder.Build();

app.MapMcp("/");
app.Run();
