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

    private TextDropDownNode? calculatorNode;

    // TODO improve this
    private readonly List<string> calculatorOptions = ["Max yield", "Max yield per GP"];

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
                Service.UIController.Update();
            }
        });

        AttachNode(new CheckboxNode
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
                Service.UIController.Update();
            }
        });

        AttachNode(new CheckboxNode
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

        AttachNode(calculatorNode = new TextDropDownNode()
        {
            IsVisible = true,
            Size = new Vector2(250, 24),
            Position = new Vector2(10, 125),
            SelectedOption = Service.Config.RotationCalculator,
            Options = calculatorOptions,
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
