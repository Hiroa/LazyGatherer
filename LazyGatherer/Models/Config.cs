using Dalamud.Configuration;

namespace LazyGatherer.Models;

public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool Display = true;
    public bool DisplayEstimatedYield = false;
}
