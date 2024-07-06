﻿using System.Drawing;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
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
            TextureHeight = 40,
            TextureWidth = 40,
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            NodeFlags = NodeFlags.Visible,
            IsVisible = true,
        };
        var iconTextureId = context.Job switch
        {
            Job.Min => action.MinerAction.Icon.ToString("000000"),
            // actionNode.Node->LoadIcon(action.Key.MinerAction.Icon); // Do not use, randomly break the icons
            Job.Bot => action.BotanistAction.Icon.ToString("000000"),
            // actionNode.Node->LoadIcon(action.Key.BotanistAction.Icon); // Do not use, randomly break the icons
            _ => "000000"
        };

        actionNode.LoadTexture($"ui/icon/001000/{iconTextureId}.tex");
        Service.NativeController.AttachToNode(actionNode, this, NodePosition.AsLastChild);

        // Frame
        frameNode = new ImageNode
        {
            NodeID = nodeId + 2,
            Color = KnownColor.White.Vector(),
            X = (44 * index),
            Size = new Vector2(48, 48),
            TextureSize = new Vector2(48, 48),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            ImageNodeFlags = 0,
            IsVisible = true,
        };
        frameNode.LoadTexture("ui/uld/IconA_Frame.tex"); // Default work fine for all styles   
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
            TextFlags = TextFlags.AutoAdjustNodeSize,
            TextFlags2 = TextFlags2.Ellipsis,
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
