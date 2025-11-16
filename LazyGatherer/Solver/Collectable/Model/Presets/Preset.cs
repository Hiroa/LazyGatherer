using System;
using System.Collections.Generic;
using Dalamud.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LazyGatherer.Solver.Collectable.Model.Presets;

public class Preset
{
    public required string Name { get; set; }
    public int MinLevel { get; set; } = 50;
    public int MinGp { get; set; }
    public List<Step> Turns { get; } = [];

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
