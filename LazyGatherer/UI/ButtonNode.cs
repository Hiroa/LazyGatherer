using System.Drawing;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Parts;
using Vector3 = System.Numerics.Vector3;

namespace LazyGatherer.UI;

public class ButtonNode : NodeBase<AtkResNode>
{
    private const uint ButtonNodeId = 7_000;
    private readonly ImageNode imageNode;
    private readonly CollisionNode collisionNode;

    public ButtonNode() : base(NodeType.Res)
    {
        var nodeId = ButtonNodeId;
        NodeID = nodeId;
        IsVisible = true;
        X = 443;
        Y = 6;
        Width = 28;
        Height = 28;
        Tooltip = "LazyGatherer Configuration";

        imageNode = new ImageNode()
        {
            NodeID = nodeId + 1,
            Color = KnownColor.White.Vector(),
            Width = 28,
            Height = 28,
            ImageNodeFlags = 0,
            IsVisible = true,
        };
        var imagePart = new Part()
        {
            Id = 5000,
            Size = new Vector2(28, 28),
            TextureCoordinates = new Vector2(0, 0)
        };

        imagePart.LoadTexture("ui/uld/fourth/CircleButtons.tex");
        imageNode.AddPart(imagePart);

        MouseOver = () => imageNode.AddColor = new Vector3(0.1f, 0.1f, 0.1f);
        MouseOut = () => imageNode.AddColor = Vector3.Zero;
        MouseClick = () => Service.ConfigController.toggleConfigWindow();

        Service.NativeController.AttachToNode(imageNode, this, NodePosition.AsFirstChild);

        collisionNode = new CollisionNode()
        {
            NodeID = nodeId + 2,
            X = 0,
            Y = 0,
            Width = 28,
            Height = 28,
            IsVisible = true,
            CollisionType = CollisionType.Move
        };
        Service.NativeController.AttachToNode(collisionNode, this, NodePosition.AsFirstChild);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            imageNode.Dispose();
            collisionNode.Dispose();
            base.Dispose(disposing);
        }
    }
}
