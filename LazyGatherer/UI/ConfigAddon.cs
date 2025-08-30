using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace LazyGatherer.UI;

public class ConfigAddon : NativeAddon
{
    private CheckboxNode displayNode = null!;
    private CheckboxNode displayYieldNode = null!;
    private CheckboxNode oneTurnNode = null!;
    private TextNode calculatorLabelNode = null!;

    private TextDropDownNode calculatorNode = null!;

    // TODO improve this
    private readonly List<String> calculatorOptions = ["Max yield", "Max yield per GP"];
    // private SliderNode gpNode = null!;

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        displayNode = new CheckboxNode()
        {
            SeString = "Display Rotation",
            IsChecked = Service.Config.Display,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(10, 45),
            OnClick = (isChecked) =>
            {
                Service.Config.Display = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update(false);
            }
        };

        displayYieldNode = new CheckboxNode()
        {
            SeString = "Display estimated yield",
            IsChecked = Service.Config.DisplayEstimatedYield,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(10, 65),
            OnClick = (isChecked) =>
            {
                Service.Config.DisplayEstimatedYield = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update(false);
            }
        };

        oneTurnNode = new CheckboxNode()
        {
            SeString = "One turn rotation",
            IsChecked = Service.Config.OneTurnRotation,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(10, 85),
            OnClick = (isChecked) =>
            {
                Service.Config.OneTurnRotation = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ReloadContext();
                Service.UIController.Update(true);
            }
        };

        calculatorLabelNode = new TextNode()
        {
            SeString = "Rotation calculator:",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(10, 105),
        };

        calculatorNode = new TextDropDownNode()
        {
            IsVisible = true,
            Size = new Vector2(250, 24),
            Position = new Vector2(10, 125),
            // MaxListOptions = 2,
            Options = calculatorOptions,
            OnOptionSelected = (selectedItem) =>
            {
                Service.Config.YieldCalculator = calculatorOptions.IndexOf(selectedItem);
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ReloadContext();
                Service.UIController.Update(true);
            },
        };

        // gpNode = new SliderNode()
        // {
        //     Size = new Vector2(250, 16),
        //     Position = new Vector2(10, 155),
        //     IsVisible = true,
        //     Min = 0,
        //     Max = (int)Service.ClientState.LocalPlayer!.MaxGp,
        //     Step = 50,
        // };
        // // Fix slider button size and position
        // gpNode.SliderForegroundButtonNode.Size = new Vector2(14.0f, 15.0f);
        // gpNode.SliderForegroundButtonNode.Position = new Vector2(0f, 1f);


        AttachNode(displayNode);
        AttachNode(displayYieldNode);
        AttachNode(oneTurnNode);
        AttachNode(calculatorLabelNode);
        AttachNode(calculatorNode);
        // AttachNode(gpNode);
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        displayNode.IsChecked = Service.Config.Display;
        displayYieldNode.IsChecked = Service.Config.DisplayEstimatedYield;
        oneTurnNode.IsChecked = Service.Config.OneTurnRotation;
        calculatorNode.SelectedOption = calculatorOptions[Service.Config.YieldCalculator];
    }
}
