using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using LazyGatherer.Models;

namespace LazyGatherer.UI.Addon;

public class ConfigAddon : NativeAddon
{
    private CheckboxNode? displayNode;

    private readonly Dictionary<ComparerEnum, string> calculatorOptions = new()
    {
        { ComparerEnum.MaxYield, "Max Yield" },
        { ComparerEnum.MaxYieldPerGp, "Max Yield per GP" }
    };

    private readonly Dictionary<EstimatedYieldStyle, string> estimatedYieldStyleOptions = new()
    {
        { EstimatedYieldStyle.Short, "Short" },
        { EstimatedYieldStyle.ShortWithMinMax, "Short with min/max" },
        { EstimatedYieldStyle.Basic, "Basic" },
        { EstimatedYieldStyle.BasicWithMinMax, "Basic with min/max" },
        { EstimatedYieldStyle.Detailed, "Detailed" },
        { EstimatedYieldStyle.DetailedWithMinMax, "Detailed with min/max" }
    };

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        SetWindowSize(new Vector2(270.0f, 284.0f));
        new TextNode
        {
            SeString = "Display options",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(10, 45),
        }.AttachNode(this);

        displayNode = new CheckboxNode
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
        };
        displayNode.AttachNode(this);

        new CheckboxNode
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
        }.AttachNode(this);

        new CheckboxNode
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
        }.AttachNode(this);

        new TextNode
        {
            SeString = "Estimated yield display style:",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(48, 125),
        }.AttachNode(this);

        new TextDropDownNode()
        {
            IsVisible = true,
            Size = new Vector2(200, 24),
            Position = new Vector2(48, 145),
            Options = estimatedYieldStyleOptions.Values.ToList(),
            SelectedOption = estimatedYieldStyleOptions[Service.Config.EstimatedYieldStyle],
            OnOptionSelected = selectedItem =>
            {
                Service.Config.EstimatedYieldStyle = estimatedYieldStyleOptions
                                                     .First(kvp => kvp.Value == selectedItem).Key;
                ;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        }.AttachNode(this);

        // Rotation options

        new TextNode
        {
            SeString = "Rotation options",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(10, 179),
        }.AttachNode(this);

        new CheckboxNode
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
        }.AttachNode(this);

        new TextNode
        {
            SeString = "Rotation calculator:",
            TextColor = ColorHelper.GetColor(8),
            TextFlags = TextFlags.Edge,
            IsVisible = true,
            Size = new Vector2(200, 20),
            FontSize = 14,
            Position = new Vector2(28, 219),
        }.AttachNode(this);

        new TextDropDownNode()
        {
            IsVisible = true,
            Size = new Vector2(220, 24),
            Position = new Vector2(28, 239),
            Options = calculatorOptions.Values.ToList(),
            SelectedOption = calculatorOptions[Service.Config.RotationCalculator],
            OnOptionSelected = selectedItem =>
            {
                Service.Config.RotationCalculator =
                    calculatorOptions.First(kvp => kvp.Value == selectedItem).Key;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        }.AttachNode(this);
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        if (displayNode != null)
            displayNode.IsChecked = Service.Config.Display;
    }
}
