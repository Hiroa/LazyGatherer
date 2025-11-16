using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using Lumina.Excel.Sheets;
using Vector3 = System.Numerics.Vector3;

namespace LazyGatherer.UI;

public sealed class ActionNode : ButtonBase
{
    private readonly IconImageNode iconNode;
    private readonly SimpleImageNode backgroundNode;
    private readonly TextNode countNode;

    public ActionNode()
    {
        // Icon node for action
        AttachNode(iconNode = new IconImageNode()
        {
            Position = new Vector2(4f, 4f),
            Size = new Vector2(40, 40),
            TextureSize = new Vector2(40, 40),
            IsVisible = true
        });

        // Background node for icon
        AttachNode(backgroundNode = new SimpleImageNode()
        {
            Position = new Vector2(0, 0),
            Size = new Vector2(48, 48),
            TextureCoordinates = new Vector2(0, 0),
            TextureSize = new Vector2(48, 48),
            IsVisible = true,
            ImageNodeFlags = 0
        });
        backgroundNode.LoadTexture("ui/uld/IconA_Frame.tex");

        // Text node for count
        AttachNode(countNode = new TextNode
        {
            AlignmentType = AlignmentType.Right,
            Position = new Vector2(4f, 4f),
            Size = new Vector2(40, 14),
            TextFlags = TextFlags.Edge,
            FontSize = 14,
        });

        InitializeComponentEvents();
    }

    public int Count
    {
        set
        {
            countNode.SeString = $"x{value}";
            countNode.IsVisible = value > 1;
        }
    }

    public bool Enabled
    {
        get => iconNode.MultiplyColor.X >= 1f;
        set => iconNode.MultiplyColor = value ? new Vector3(1f) : new Vector3(0.5f);
    }

    public uint ActionId
    {
        set
        {
            var action = Service.DataManager.Excel.GetSheet<Action>()[value];
            iconNode.LoadIcon(action.Icon);
            Tooltip = action.Name.ToDalamudString().TextValue;
        }
    }

    public bool CanBeToggle
    {
        set { OnClick = value ? () => Enabled = !Enabled : null; }
    }

    private void AttachNode(NodeBase node)
    {
        Service.NativeController.AttachNode(node, this, NodePosition.AsLastChild);
    }
}
