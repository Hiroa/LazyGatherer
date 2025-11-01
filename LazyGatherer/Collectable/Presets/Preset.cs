using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Utility;
using LazyGatherer.Collectable.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LazyGatherer.Collectable.Presets;

public class Preset
{
    public required string Name { get; set; }

    public int RecommendedGp { get; set; }
    public List<Step> Steps { get; } = [];

    private int MinLevelNeeded()
    {
        var minLvlNeeded = 0;
        foreach (var step in Steps)
        {
            var max = step.Actions
                          .Select(a => CollectableAction.Actions[a].Level)
                          .Max();
            minLvlNeeded = Math.Max(minLvlNeeded, max);
        }

        return minLvlNeeded;
    }

    /*
     * Exports the preset.
     * JSON serialized, compressed and Base64.
     */
    public string Export()
    {
        var presetStr = JsonConvert.SerializeObject(this, JsonSettings);
        return Convert.ToBase64String(Util.CompressString(presetStr));
    }

    /*
     * Imports a preset from a string.
     * Decodes from Base64, decompressed and JSON deserialized.
     */
    public static Preset Import(string str)
    {
        try
        {
            var importStr = Util.DecompressString(Convert.FromBase64String(str));
            var preset = JsonConvert.DeserializeObject<Preset>(importStr, JsonSettings);
            if (preset == null)
            {
                Service.Log.Warning("Imported preset was null.");
                return new Preset
                {
                    Name = "New Preset"
                };
            }

            return preset;
        }
        catch (Exception ex)
        {
            Service.Log.Warning($"Error importing preset:\n{ex}");
            return new Preset
            {
                Name = "New Preset"
            };
        }
    }

    internal static readonly JsonSerializerSettings JsonSettings = new()
    {
        Converters =
        {
            new BaseActionConverter(),
            new StringEnumConverter()
        }
    };
}
