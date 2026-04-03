using System.Text;
using System.Text.Json;

namespace MtaWikiMcp.Common.Services;

/// <summary>
/// Provides static game data loaded from the bundled JSON data files (peds, vehicles).
/// </summary>
public class GameDataProvider
{
    private readonly IReadOnlyDictionary<string, int> _peds;
    private readonly IReadOnlyDictionary<string, int> _vehicles;

    public GameDataProvider()
    {
        _peds = LoadJson("Data/peds.json");
        _vehicles = LoadJson("Data/vehicles.json");
    }

    private static IReadOnlyDictionary<string, int> LoadJson(string relativePath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, relativePath);
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(json)!;
    }

    public string GetPedModelList()
    {
        var sb = new StringBuilder();
        foreach (var (name, id) in _peds.OrderBy(x => x.Value))
            sb.AppendLine($"{id}: {name}");
        return sb.ToString();
    }

    public string GetVehicleModelList()
    {
        var sb = new StringBuilder();
        foreach (var (name, id) in _vehicles.OrderBy(x => x.Value))
            sb.AppendLine($"{id}: {name}");
        return sb.ToString();
    }
}
