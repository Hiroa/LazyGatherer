using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Slider;
using KamiToolKit.System;

namespace LazyGatherer.UI;

public class GpSliderNode : CustomNode
{
    private readonly SliderNode sliderNode;

    public GpSliderNode(int maxGp)
    {
        AttachNode(new TextNode()
        {
            Position = Vector2.Zero,
            Size = new Vector2(200, 12),
            IsVisible = true,
            String = "Max GP to use on rotation",
            FontSize = 12,
            FontType = FontType.Axis,
            TextColor = ColorHelper.GetColor(2)
        });
        // Slider node for GP
        AttachNode(sliderNode = new SliderNode()
        {
            Position = new Vector2(0, 8),
            Size = new Vector2(150, 28),
            IsVisible = true,
            Step = 50,
            Max = maxGp,
            Min = 0,
            Value = maxGp
        });
    }

    private void AttachNode(NodeBase node)
    {
        Service.NativeController.AttachNode(node, this, NodePosition.AsLastChild);
    }
}
