# Project Overview

This repositories contains the source for MCP server(s?) intended to augment AI tooling use for creating resources / scripts for MTA San Andreas.

## MCPs and tools
These are the tools that the MCP server exposes:

## Wiki MCP
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

### Usage
The easiest way to use the MCP is by adding the MTA-hosted version of the MCP to your mcp.json (or your IDE's equivalent)
```json
"MTA Wiki MCP": {
	"url": "https://mcp.multitheftauto.com/wiki",
	"type": "http"
}
```

## Debug companion MCP

The Debug Companion MCP exposes tools to run Lua on a running MTA server, control resources, and fetch recent server logs.

Available tools:
- `RunCode(string code)`  
  Runs arbitrary Lua code on the server and returns the result. For multi-statement results return a value via a `return` (for example, wrap in an immediately-invoked function and `return toJSON(...)`).
- `RestartResource(string name)`  
  Restarts the specified resource.
- `StartResource(string name)`  
  Starts the specified resource.
- `StopResource(string name)`  
  Stops the specified resource.
- `GetLogs()`  
  Retrieves the latest ~100 lines of debug logs.

### Usage

- Run the `debugCompanion` resource on the MTA server.
  - You might need to give `user.*` access to `resource.debugCompanion.http` in ACL in order for the MCP server to be able to access the companion resource.
- If you changed the default hostname or API key, make sure to update the values in the project's `appsettings.json` / `appsettings.local.json` so the MCP can communicate with the server.
- Run the MCP locally (the HTTP MCP exposes the service on the local host).
- Add the MCP to your `mcp.json` (or IDE equivalent) pointing at the local URL (default local port used by the HTTP host is 5277):

```json
"Mta Debug Companion MCP": {
   "url": "http://localhost:5277",
   "type": "http"
}
```

Once added you can invoke the Debug Companion tools from your agent to run code, manage resources, and fetch logs.  

You can use this to let the agent iterate independently, changing code, applying it, and verifying the changes work as intended.  
It is recommended to set up a `.github/copilot-instructions.md` file in your workspace, or whatever is the equivalent for the tool you use, in order to make sure the models know how to use the MCPs.

## Development
The MCP server is a C# dotnet 10 application, developing requires dotnet 10 installed, and can be done in Visual Studio, vscore, or any IDE of your choosing.  

The codebase has several projects:
- MtaWikiMcp.Common  
  The common library which contains the actual tools that are exposed via the MCP server.
- MtaWikiMcp.Http  
  A version of the MCP server that runs via HTTP.
- MtaWikiMcp.Stdio  
  A version of the MCP server that can be executed locally via stdio if you don't want to use HTTP. Do note this will still require access to the wiki.
