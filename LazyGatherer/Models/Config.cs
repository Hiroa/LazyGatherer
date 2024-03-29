using Dalamud.Configuration;
using KamiLib.AutomaticUserInterface;

namespace LazyGatherer.Models;

[Category("Display", -1)]
public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    [BoolConfig("Enable")]
    public bool Display { get; set; } = true;
    [BoolConfig("Display estimated yield")]
    public bool DisplayEstimatedYield { get; set; } = false;

    public bool AsChanged { get; set; }
}
