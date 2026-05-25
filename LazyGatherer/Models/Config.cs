using Dalamud.Configuration;
using Dalamud.Game.Text;

namespace LazyGatherer.Models;

public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    // Gathering Settings
    public bool Display = true;
    public bool DisplayGpSlider = true;
    public bool DisplayEstimatedYield = false;
    public ComparerEnum RotationCalculator = ComparerEnum.MaxYield;
    public EstimatedYieldStyle EstimatedYieldStyle = EstimatedYieldStyle.Basic;
    public bool OneTurnRotation = false;

    // Masterpiece Settings
    public bool CollectableDisplay = true;
    public string CollectableLastRotation = "";

    // GP Alert
    public bool GpAlertEnabled = false;
    public uint GpAlertThreshold = 0;
    public uint GpAlertSound = 1;
    public bool GpAlertSendChatMessage = false;
    public XivChatType GpAlertChatType = XivChatType.Echo;
}
