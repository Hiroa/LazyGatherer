using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
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

    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan)
    {
        SetWindowSize(new Vector2(300.0f, 332.0f));
        var displayTextNode = new TextNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = ContentStartPosition with { Y = 45 },
            String = "Display options",
            TextFlags = TextFlags.Edge,
            FontSize = 14,
        };
        displayTextNode.AttachNode(this);

        displayNode = new CheckboxNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(ContentStartPosition.X + 16, displayTextNode.Bounds.Bottom),
            String = "Display Rotation",
            IsChecked = Service.Config.Display,
            OnClick = isChecked =>
            {
                Service.Config.Display = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        };
        displayNode.AttachNode(this);

        var displayGpSliderCb = new CheckboxNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(displayNode.X, displayNode.Bounds.Bottom),
            String = "Display max GP slider",
            IsChecked = Service.Config.DisplayGpSlider,
            OnClick = isChecked =>
            {
                Service.Config.DisplayGpSlider = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        };
        displayGpSliderCb.AttachNode(this);

        var displayBackgroundCb = new CheckboxNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(displayNode.X, displayGpSliderCb.Bounds.Bottom),
            String = "Display background",
            IsChecked = Service.Config.DisplayBackground,
            OnClick = isChecked =>
            {
                Service.Config.DisplayBackground = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        };
        displayBackgroundCb.AttachNode(this);

        var displayEstimatedYieldCb = new CheckboxNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(displayBackgroundCb.X, displayBackgroundCb.Bounds.Bottom),
            String = "Display estimated yield",
            IsChecked = Service.Config.DisplayEstimatedYield,
            OnClick = isChecked =>
            {
                Service.Config.DisplayEstimatedYield = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        };
        displayEstimatedYieldCb.AttachNode(this);

        var estimatedYieldTextNode = new TextNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(displayEstimatedYieldCb.X + 20, displayEstimatedYieldCb.Bounds.Bottom),
            String = "Estimated yield display style:",
            FontSize = 14,
        };
        estimatedYieldTextNode.AttachNode(this);

        var estimatedYieldDropDownNode = new TextDropDownNode
        {
            IsVisible = true,
            Size = new Vector2(230, 24),
            // Position = new Vector2(48, 145),
            Position = new Vector2(estimatedYieldTextNode.X, estimatedYieldTextNode.Bounds.Bottom),
            Options = estimatedYieldStyleOptions.Values.ToList(),
            SelectedOption = estimatedYieldStyleOptions[Service.Config.EstimatedYieldStyle],
            OnOptionSelected = selectedItem =>
            {
                Service.Config.EstimatedYieldStyle = estimatedYieldStyleOptions
                                                     .First(kvp => kvp.Value == selectedItem).Key;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        };
        estimatedYieldDropDownNode.AttachNode(this);

        var separatorNode = new HorizontalLineNode
        {
            Position = ContentStartPosition with { Y = estimatedYieldDropDownNode.Bounds.Bottom },
            Size = new Vector2(ContentSize.X, 4),
        };
        separatorNode.AttachNode(this);

        // Rotation options

        var rotationOptionsTextNode = new TextNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = ContentStartPosition with { Y = separatorNode.Bounds.Bottom + 5 },
            String = "Rotation options",
            TextFlags = TextFlags.Edge,
            FontSize = 14,
        };
        rotationOptionsTextNode.AttachNode(this);

        var onTurnRoationCb = new CheckboxNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(ContentStartPosition.X + 16, rotationOptionsTextNode.Bounds.Bottom),
            String = "One turn rotation",
            IsChecked = Service.Config.OneTurnRotation,
            OnClick = isChecked =>
            {
                Service.Config.OneTurnRotation = isChecked;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            }
        };
        onTurnRoationCb.AttachNode(this);

        new SimpleImageNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(ContentSize.X - 25, onTurnRoationCb.Y - 5),
            TexturePath = "ui/uld/CircleButtons.tex",
            TextureSize = new Vector2(28.0f, 28.0f),
            TextureCoordinates = new Vector2(112.0f, 84.0f),
            TextTooltip = "Force solver to compute a rotation that do not use actions after the first gathering.\n" +
                          "This may result in suboptimal rotations, but can be useful for quick gathering.",
        }.AttachNode(this);

        var rotationCalculatorTextNode = new TextNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 20 },
            Position = new Vector2(ContentStartPosition.X + 16, onTurnRoationCb.Bounds.Bottom),
            String = "Rotation calculator:",
            FontSize = 14,
        };
        rotationCalculatorTextNode.AttachNode(this);

        var rotationCalculatorDropDown = new TextDropDownNode
        {
            IsVisible = true,
            Size = new Vector2(250, 24),
            Position = new Vector2(rotationCalculatorTextNode.X, rotationCalculatorTextNode.Bounds.Bottom),
            Options = calculatorOptions.Values.ToList(),
            SelectedOption = calculatorOptions[Service.Config.RotationCalculator],
            OnOptionSelected = selectedItem =>
            {
                Service.Config.RotationCalculator =
                    calculatorOptions.First(kvp => kvp.Value == selectedItem).Key;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.GatheringController.ComputeRotations();
            },
        };
        rotationCalculatorDropDown.AttachNode(this);

        separatorNode = new HorizontalLineNode
        {
            Position = ContentStartPosition with { Y = rotationCalculatorDropDown.Bounds.Bottom + 5 },
            Size = ContentSize with { Y = 4 },
        };
        separatorNode.AttachNode(this);

        var gpAlertButton = new TextButtonNode
        {
            IsVisible = true,
            Size = ContentSize with { Y = 24 },
            Position = ContentStartPosition with { Y = separatorNode.Bounds.Bottom + 5 },
            String = "GP Alert configuration",
            OnClick = () => Service.GpAlertAddon.Toggle()
        };
        gpAlertButton.AttachNode(this);
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        if (displayNode != null)
            displayNode.IsChecked = Service.Config.Display;
    }
}
