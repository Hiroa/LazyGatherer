using System.Drawing;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Parts;
using LazyGatherer.Solver.Actions;
using LazyGatherer.Solver.Data;

namespace LazyGatherer.UI;

public class IconNode : NodeBase<AtkResNode>
{
    private const uint IconNodeId = 6_100;
    private readonly ImageNode actionNode;
    private readonly ImageNode frameNode;
    private readonly TextNode countNode;

    public IconNode(GatheringContext context, BaseAction action, int count, uint index) : base(NodeType.Res)
    {
        var nodeId = IconNodeId + (index * 10);
        NodeID = nodeId;
        IsVisible = true;

        // Action
        actionNode = new ImageNode
        {
            NodeID = nodeId + 1,
            Color = KnownColor.White.Vector(),
            X = 4f + (44 * index),
            Y = 4f,
            Width = 40,
            Height = 40,
            NodeFlags = NodeFlags.Visible,
            IsVisible = true,
        };
        var actionPart = new Part()
        {
            Size = new Vector2(40, 40),
            TextureCoordinates = Vector2.Zero
        };

        var iconTextureId = context.Job switch
        {
            Job.Min => action.MinerAction.Icon.ToString("000000"),
            // actionNode.Node->LoadIcon(action.Key.MinerAction.Icon); // Do not use, randomly break the icons
            Job.Bot => action.BotanistAction.Icon.ToString("000000"),
            // actionNode.Node->LoadIcon(action.Key.BotanistAction.Icon); // Do not use, randomly break the icons
            _ => "000000"
        };
        actionPart.LoadTexture($"ui/icon/001000/{iconTextureId}.tex");
        actionNode.AddPart(actionPart);

        Service.NativeController.AttachToNode(actionNode, this, NodePosition.AsLastChild);

        // Frame
        frameNode = new ImageNode
        {
            NodeID = nodeId + 2,
            Color = KnownColor.White.Vector(),
            X = (44 * index),
            Size = new Vector2(48, 48),
            ImageNodeFlags = 0,
            IsVisible = true,
        };
        var framePart = new Part()
        {
            Size = new Vector2(48, 48),
            TextureCoordinates = Vector2.Zero
        };
        framePart.LoadTexture("ui/uld/IconA_Frame.tex"); // Default work fine for all styles   
        frameNode.AddPart(framePart);

        Service.NativeController.AttachToNode(frameNode, this, NodePosition.AsLastChild);

        //Count
        countNode = new TextNode
        {
            NodeID = nodeId + 3,
            AlignmentType = AlignmentType.Right,
            X = 4f + (44 * index),
            Y = 4f,
            Width = 40,
            Height = 14,
            Text = $"x{count}",
            FontType = FontType.Axis,
            TextFlags = TextFlags.Edge,
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = KnownColor.Black.Vector(),
            FontSize = 14,
            IsVisible = count > 1
        };
        Service.NativeController.AttachToNode(countNode, this, NodePosition.AsLastChild);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            actionNode.Dispose();
            frameNode.Dispose();
            countNode.Dispose();
            base.Dispose(disposing);
        }
    }
}
