using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using LazyGatherer.Solver.Models;

namespace LazyGatherer.UI;

public class ConfigAddon : NativeAddon
{
    private CheckboxNode? displayNode;

    private TextDropDownNode? calculatorNode;
    private TextDropDownNode? EstimatedYieldStyleNode;

    // TODO improve this
    private readonly List<string> calculatorOptions = ["Max yield", "Max yield per GP"];
    private readonly List<string> estimatedYieldStyleOptions = [..Enum.GetNames<EstimatedYieldStyle>()];

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        AttachNode(new TextNode
        {
            SeString = "Display options",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(10, 45),
        });

        AttachNode(displayNode = new CheckboxNode
        {
            SeString = "Display Rotation",
            IsChecked = Service.Config.Display,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(28, 65),
            OnClick = isChecked =>
            {
                Service.Config.Display = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        });

        AttachNode(new CheckboxNode
        {
            SeString = "Display max GP slider",
            IsChecked = Service.Config.DisplayGpSlider,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(28, 85),
            OnClick = isChecked =>
            {
                Service.Config.DisplayGpSlider = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        });

        AttachNode(new CheckboxNode
        {
            SeString = "Display estimated yield",
            IsChecked = Service.Config.DisplayEstimatedYield,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(28, 105),
            OnClick = isChecked =>
            {
                Service.Config.DisplayEstimatedYield = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        });

        AttachNode(new TextNode
        {
            SeString = "Estimated yield display style:",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(48, 125),
        });

        AttachNode(EstimatedYieldStyleNode = new TextDropDownNode()
        {
            IsVisible = true,
            Size = new Vector2(200, 24),
            Position = new Vector2(48, 145),
            Options = estimatedYieldStyleOptions,
            SelectedOption = Service.Config.EstimatedYieldStyle.ToString(),
            OnOptionSelected = selectedItem =>
            {
                Service.Config.EstimatedYieldStyle = Enum.Parse<EstimatedYieldStyle>(selectedItem);
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        });

        // Rotation options

        AttachNode(new TextNode
        {
            SeString = "Rotation options",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(10, 179),
        });

        AttachNode(new CheckboxNode
        {
            SeString = "One turn rotation",
            IsChecked = Service.Config.OneTurnRotation,
            IsVisible = true,
            Size = new Vector2(150, 20),
            Position = new Vector2(28, 199),
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
            Position = new Vector2(28, 219),
        });

        AttachNode(calculatorNode = new TextDropDownNode()
        {
            IsVisible = true,
            Size = new Vector2(220, 24),
            Position = new Vector2(28, 239),
            Options = calculatorOptions,
            SelectedOption = Service.Config.RotationCalculator,
            OnOptionSelected = selectedItem =>
            {
                Service.Config.RotationCalculator = selectedItem;
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
    }
}
