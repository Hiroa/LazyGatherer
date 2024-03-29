using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.AutomaticUserInterface;
using LazyGatherer.Models;

namespace LazyGatherer.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Config config;

    public ConfigWindow(Config config) : base("LazyGatherer - Configuration")
    {
        this.config = config;
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
        DrawableAttribute.DrawAttributes(config, () =>
        {
            Service.Interface.SavePluginConfig(config);
            config.AsChanged = true;
        });
    }
}
