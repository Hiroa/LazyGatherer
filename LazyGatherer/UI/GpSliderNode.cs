using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace LazyGatherer.UI;

public class GpSliderNode : SimpleComponentNode
{
    private readonly SliderNode sliderNode;

    public GpSliderNode(int maxGp)
    {
        new TextNode()
        {
            Position = Vector2.Zero,
            Size = new Vector2(200, 12),
            IsVisible = true,
            String = "Max GP to use on rotation",
            FontSize = 12,
            FontType = FontType.Axis,
            TextColor = ColorHelper.GetColor(2)
        }.AttachNode(this);
        // Slider node for GP
        sliderNode = new SliderNode()
        {
            Position = new Vector2(0, 14),
            Size = new Vector2(150, 20),
            IsVisible = true,
            Step = 50,
            Range = new Range(0, maxGp),
            Value = maxGp,
            OnValueChanged = value => { Service.GatheringController.ComputeRotations(value); }
        };
        // Reshape slider for aesthetics
        sliderNode.SliderBackgroundButtonNode.Height = 18;
        sliderNode.SliderBackgroundButtonNode.Width = 150 - 22;
        sliderNode.SliderBackgroundButtonNode.Y = 0;
        sliderNode.SliderForegroundButtonNode.Y = 1.5f;
        sliderNode.AttachNode(this);
    }
}
