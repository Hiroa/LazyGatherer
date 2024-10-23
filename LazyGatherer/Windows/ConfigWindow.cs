using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace LazyGatherer.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow() : base("LazyGatherer - Configuration")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(300, 200)
        };
        this.SizeCondition = ImGuiCond.Always;
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (!IsOpen) return;
        var configChanged = false;
        var rotationConfigChanged = false;
        configChanged |= ImGui.Checkbox("Display", ref Service.Config.Display);
        configChanged |= ImGui.Checkbox("Display expected yield", ref Service.Config.DisplayEstimatedYield);
        rotationConfigChanged |= ImGui.Checkbox("One turn rotation", ref Service.Config.OneTurnRotation);
        rotationConfigChanged |= ImGui.Combo("Gathering calculator", ref Service.Config.YieldCalculator,
                                             ["Max yield", "Max yield per GP"], 2);
        if (configChanged)
        {
            Service.Interface.SavePluginConfig(Service.Config);
            Service.UIController.Update(false);
        }

        if (rotationConfigChanged)
        {
            Service.Interface.SavePluginConfig(Service.Config);
            Service.GatheringController.Update();
            Service.UIController.Update(true);
        }
    }
}
