using Dalamud.Configuration;
using LazyGatherer.Solver.Models;

namespace LazyGatherer.Models;

public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool Display = true;
    public bool DisplayGpSlider = true;
    public bool DisplayEstimatedYield = false;
    public string RotationCalculator = "Max yield";
    public EstimatedYieldStyle EstimatedYieldStyle = EstimatedYieldStyle.Basic;
    public bool OneTurnRotation = false;
}
