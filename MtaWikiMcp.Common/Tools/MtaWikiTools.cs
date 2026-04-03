using ModelContextProtocol.Server;
using MtaWikiMcp.Common.Clients;
using MtaWikiMcp.Common.Services;
using System.ComponentModel;

namespace MtaWikiMcp.Common.Tools;

public enum ScriptSide { Server, Client, Shared }

/// <summary>
/// This class contains the tools that are exposed to the AI agent for accessing information from the MTA wiki
/// </summary>
/// <param name="wikiScraper"></param>
[McpServerToolType]
[Description("Tools that allow access to MTA's Lua function information from the MTA wiki")]
public class MtaWikiTools(WikiClient wikiScraper, GameDataProvider gameData)
{
    private const string MtaLogoUrl = "https://wiki.multitheftauto.com/images/thumb/5/58/Mtalogo.png/100px-Mtalogo.png";

    [McpServerTool(Name = nameof(GetFunctionList), IconSource = MtaLogoUrl)]
    [Description("Returns a list of all available MTA scripting functions for the specified side (Server, Client, or Shared), grouped by category")]
    public Task<string> GetFunctionList(ScriptSide side) => side switch
    {
        ScriptSide.Server => wikiScraper.GetFunctionListAsync("Server_Scripting_Functions"),
        ScriptSide.Client => wikiScraper.GetFunctionListAsync("Client_Scripting_Functions"),
        ScriptSide.Shared => wikiScraper.GetFunctionListAsync("Shared_Scripting_Functions"),
        _ => Task.FromResult("Invalid side.")
    };

    [McpServerTool(Name = nameof(GetFunctionInformation), IconSource = MtaLogoUrl)]
    [Description("Returns the description, syntax, parameters and example for a specific MTA Wiki function")]
    public Task<string> GetFunctionInformation(string functionName) =>
        wikiScraper.GetFunctionInformationAsync(functionName);


    [McpServerTool(Name = nameof(GetEventList), IconSource = MtaLogoUrl)]
    [Description("Returns a list of all available MTA scripting events for the specified side (Server or Client), grouped by category")]
    public Task<string> GetEventList(ScriptSide side) => side switch
    {
        ScriptSide.Server => wikiScraper.GetFunctionListAsync("Server_Scripting_Events"),
        ScriptSide.Client => wikiScraper.GetFunctionListAsync("Client_Scripting_Events"),
        ScriptSide.Shared => Task.FromResult("There are no shared scripting events."),
        _ => Task.FromResult("Invalid side.")
    };

    [McpServerTool(Name = nameof(GetEventParameters), IconSource = MtaLogoUrl)]
    [Description("Returns the parameters and event source for a specific MTA Wiki event")]
    public Task<string> GetEventParameters(string eventName) =>
        wikiScraper.GetEventParametersAsync(eventName);


    [McpServerTool(Name = nameof(GetPedModels), IconSource = MtaLogoUrl)]
    [Description("Returns a list of all valid GTA:SA ped model IDs and their names")]
    public Task<string> GetPedModels() =>
        Task.FromResult(gameData.GetPedModelList());

    [McpServerTool(Name = nameof(GetVehicleModels), IconSource = MtaLogoUrl)]
    [Description("Returns a list of all valid GTA:SA vehicle model IDs and their names")]
    public Task<string> GetVehicleModels() =>
        Task.FromResult(gameData.GetVehicleModelList());


    [McpServerTool(Name = nameof(SearchWiki), IconSource = MtaLogoUrl)]
    [Description("Searches the MTA Wiki for pages matching the given query. Returns a list of matching page names (usable with GetFunctionInformation and GetPageSource) and their short descriptions")]
    public Task<string> SearchWiki(string query) =>
        wikiScraper.SearchWikiAsync(query);

    [McpServerTool(Name = nameof(GetPageSource), IconSource = MtaLogoUrl)]
    [Description("Returns the raw HTML source of a MTA Wiki page, useful as a fallback for detailed parsing when other tools do not return the expected result")]
    public Task<string> GetPageSource(string functionName) =>
        wikiScraper.GetRawPageAsync(functionName);
}
