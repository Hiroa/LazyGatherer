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
    private CheckboxNode? displayNode;
    private CheckboxNode? displayYieldNode;
    private CheckboxNode? oneTurnNode;

    private TextDropDownNode? calculatorNode;

    // TODO improve this
    private readonly List<String> calculatorOptions = ["Max yield", "Max yield per GP"];

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        AttachNode(displayNode = new CheckboxNode
        {
            SeString = "Display Rotation",
            IsChecked = Service.Config.Display,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(10, 45),
            OnClick = isChecked =>
            {
                Service.Config.Display = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.UpdateRotations();
            }
        });

        AttachNode(displayYieldNode = new CheckboxNode
        {
            SeString = "Display estimated yield",
            IsChecked = Service.Config.DisplayEstimatedYield,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(10, 65),
            OnClick = isChecked =>
            {
                Service.Config.DisplayEstimatedYield = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.UpdateRotations();
            }
        });

        AttachNode(oneTurnNode = new CheckboxNode
        {
            SeString = "One turn rotation",
            IsChecked = Service.Config.OneTurnRotation,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(10, 85),
            OnClick = isChecked =>
            {
                Service.Config.OneTurnRotation = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            }
        });

        AttachNode(new TextNode
        {
            SeString = "Rotation calculator:",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(10, 105),
        });

        AttachNode(calculatorNode = new TextDropDownNode
        {
            IsVisible = true,
            Size = new Vector2(250, 24),
            Position = new Vector2(10, 125),
            Options = calculatorOptions,
            OnOptionSelected = selectedItem =>
            {
                Service.Config.YieldCalculator = calculatorOptions.IndexOf(selectedItem);
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        });
        calculatorNode.OptionListNode.ScrollBarNode.IsVisible = false;
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        if (displayNode != null)
            displayNode.IsChecked = Service.Config.Display;
        if (displayYieldNode != null)
            displayYieldNode.IsChecked = Service.Config.DisplayEstimatedYield;
        if (oneTurnNode != null)
            oneTurnNode.IsChecked = Service.Config.OneTurnRotation;
        if (calculatorNode != null)
            calculatorNode.SelectedOption = calculatorOptions[Service.Config.YieldCalculator];
    }
}
