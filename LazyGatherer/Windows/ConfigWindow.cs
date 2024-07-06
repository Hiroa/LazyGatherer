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
        configChanged |= ImGui.Checkbox("Display", ref Service.Config.Display);
        configChanged |= ImGui.Checkbox("Display expected yield", ref Service.Config.DisplayEstimatedYield);
        if (configChanged)
        {
            Service.Interface.SavePluginConfig(Service.Config);
            Service.UIController.Update();
        }
    }
}
