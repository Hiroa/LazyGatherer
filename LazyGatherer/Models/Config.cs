using Dalamud.Configuration;

namespace LazyGatherer.Models;

public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool Display = true;
    public bool DisplayGpSlider = true;
    public bool DisplayEstimatedYield = false;
    public ComparerEnum RotationCalculator = ComparerEnum.MaxYield;
    public EstimatedYieldStyle EstimatedYieldStyle = EstimatedYieldStyle.Basic;
    public bool OneTurnRotation = false;
}
