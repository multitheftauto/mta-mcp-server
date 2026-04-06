using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MtaDebugCompanionMcp.Common.Clients;

public class MtaServerDebugClient(HttpClient httpClient)
{
    private static StringContent ToJsonContent<T>(T value) =>
        new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

    public async Task<string> RunCode(string code)
    {
        var response = await httpClient.PostAsync("/debugCompanion/call/httpRun", ToJsonContent(new[] { code }));
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> RestartResource(string name)
    {
        var response = await httpClient.PostAsync("/debugCompanion/call/httpRestartResource", ToJsonContent(new[] { name }));
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> StartResource(string name)
    {
        var response = await httpClient.PostAsync("/debugCompanion/call/httpStartResource", ToJsonContent(new[] { name }));
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> StopResource(string name)
    {
        var response = await httpClient.PostAsync("/debugCompanion/call/httpStopResource", ToJsonContent(new[] { name }));
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetLogs()
    {
        var response = await httpClient.PostAsync("/debugCompanion/call/httpGetDebugLog", ToJsonContent(Array.Empty<string>()));
        return await response.Content.ReadAsStringAsync();
    }
}
