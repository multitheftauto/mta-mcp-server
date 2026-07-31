using ModelContextProtocol.Server;
using MtaDebugCompanionMcp.Common.Clients;
using System.ComponentModel;

namespace MtaDebugCompanionMcp.Common.Tools;

public enum ScriptSide { Server, Client, Shared }

/// <summary>
/// This class contains the tools that are exposed to the AI agent for accessing information from the MTA wiki
/// </summary>
/// <param name="wikiScraper"></param>
[McpServerToolType]
[Description("Tools that allow access certain actions on the MTA server")]
public class MtaDebugCompanionTools(MtaServerDebugClient mtaServer)
{
    private const string MtaLogoUrl = "https://wiki.multitheftauto.com/images/thumb/5/58/Mtalogo.png/100px-Mtalogo.png";

    [McpServerTool(Name = nameof(RunCode), IconSource = MtaLogoUrl)]
    [Description("""
        Runs any arbitrary Lua code on the MTA server, for the purpose of debugging. Will return any result from the code that was run. 
        If running more than a single statement returned information can be obtained using:
        (function()
            -- code here
            return toJSON({ --[[ any value here ]] })
        end)()
        """)]
    public Task<string> RunCode(string code)
    {
        return mtaServer.RunCode(code);
    }

    [McpServerTool(Name = nameof(RestartResource), IconSource = MtaLogoUrl)]
    [Description("Restarts the specified resource on the MTA server.")]
    public Task<string> RestartResource(string name)
    {
        return mtaServer.RestartResource(name);
    }

    [McpServerTool(Name = nameof(StartResource), IconSource = MtaLogoUrl)]
    [Description("Starts the specified resource on the MTA server.")]
    public Task<string> StartResource(string name)
    {
        return mtaServer.StartResource(name);
    }

    [McpServerTool(Name = nameof(StopResource), IconSource = MtaLogoUrl)]
    [Description("Stops the specified resource on the MTA server.")]
    public Task<string> StopResource(string name)
    {
        return mtaServer.StopResource(name);
    }

    [McpServerTool(Name = nameof(GetLogs), IconSource = MtaLogoUrl)]
    [Description("Retrieves the latest 100 lines of debug logs.")]
    public Task<string> GetLogs()
    {
        return mtaServer.GetLogs();
    }


}
