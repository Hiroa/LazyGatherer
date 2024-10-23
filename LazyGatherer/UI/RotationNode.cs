using System;
using System.Collections.Generic;
using System.Drawing;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using LazyGatherer.Solver.Actions;
using LazyGatherer.Solver.Data;

namespace LazyGatherer.UI;

public class RotationNode : NodeBase<AtkResNode>
{
    private const uint ContainerNodeId = 6000;
    private const uint Marging = 4;

    private readonly ListNode<IconNode> actionsNode;
    private readonly TextNode expectedYieldNode;

    public RotationNode(KeyValuePair<Rotation, GatheringOutcome> outcome) : base(NodeType.Res)
    {
        NodeID = ContainerNodeId;
        Position = new Vector2(496.0f, 68f + (44.0f * outcome.Key.Context.RowId));
        IsVisible = Service.Config.Display;
        actionsNode = new ListNode<IconNode>
        {
            IsVisible = true,
            LayoutOrientation = LayoutOrientation.Horizontal,
            NodeID = ContainerNodeId + 1,
            Color = KnownColor.White.Vector(),
            // Position = new Vector2(500.0f, 68f + (44.0f * outcome.Key.Context.RowId)),
            Margin = new Spacing(Marging),
            BackgroundVisible = false,
        };

        Service.NativeController.AttachToNode(actionsNode, this, NodePosition.AsLastChild);

        var context = outcome.Key.Context;
        var kv = this.CompactActions(outcome.Key.Actions);
        var index = 0u;
        foreach (var action in kv)
        {
            var iconNode = new IconNode(context, action.Key, action.Value, index);
            actionsNode.Add(iconNode);
            index++;
        }

        expectedYieldNode = new TextNode
        {
            NodeID = ContainerNodeId + 2,
            Position = new Vector2(Marging + (44 * index), 26),
            Size = new Vector2(20),
            FontType = FontType.Axis,
            TextFlags = TextFlags.Edge,
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = KnownColor.Black.Vector(),
            // AlignmentType = AlignmentType.BottomRight,
            FontSize = 14,
            Text = $"Expected yield: {Math.Round(outcome.Value.Yield, 1)} for {outcome.Value.UsedGp} GP",
            IsVisible = true,
        };

        Service.NativeController.AttachToNode(expectedYieldNode, this, NodePosition.AsLastChild);
    }

    private Dictionary<BaseAction, int> CompactActions(List<BaseAction> actions)
    {
        var dictionary = new Dictionary<BaseAction, int>();
        foreach (var action in actions)
        {
            if (!dictionary.TryAdd(action, 1))
            {
                dictionary[action] += 1;
            }
        }

        return dictionary;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            actionsNode.Dispose();
            expectedYieldNode.Dispose();
            base.Dispose(disposing);
        }
    }

    public void Update()
    {
        var config = Service.Config;
        IsVisible = config.Display;
        expectedYieldNode.IsVisible = config.DisplayEstimatedYield;
    }
}
