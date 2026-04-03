# Project Overview

This repositories contains the source for MCP server(s?) intended to augment AI tooling use for creating resources / scripts for MTA San Andreas.

## MCPs and tools
These are the tools that the MCP server exposes:

- `GetFunctionList(side)`  
   lists MTA scripting functions grouped by category (`Server`, `Client`, `Shared`).
- `GetFunctionInformation(functionName)`  
   returns description, syntax, parameters and example for a wiki function.
- `GetEventList(side)`  
   lists scripting events for `Server` or `Client` sides.
- `GetEventParameters(eventName)`  
   returns parameters and source information for a specific event.
- `GetPedModels()`  
   returns bundled GTA:SA ped model IDs and names.
- `GetVehicleModels()`  
   returns bundled GTA:SA vehicle model IDs and names.
- `SearchWiki(query)`  
   searches the MTA Wiki and returns matching pages usable with the other tools.
- `GetPageSource(functionName)`  
   returns raw HTML source for a wiki page.

## Usage
The easiest way to use the MCP is by adding the MTA-hosted version of the MCP to your mcp.json (or your IDE's equivalent)
```json
"MTA Wiki MCP": {
	"url": "https://mcp.multitheftauto.com/wiki",
	"type": "http"
}
```

## Development
The MCP server is a C# dotnet 10 application, developing requires dotnet 10 installed, and can be done in Visual Studio, vscore, or any IDE of your choosing.  

The codebase has several projects:
- MtaWikiMcp.Common  
  The common library which contains the actual tools that are exposed via the MCP server.
- MtaWikiMcp.Http  
  A version of the MCP server that runs via HTTP.
- MtaWikiMcp.Stdio  
  A version of the MCP server that can be executed locally via stdio if you don't want to use HTTP. Do note this will still require access to the wiki.
