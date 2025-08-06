using System;
using System.Collections.Generic;
using System.Drawing;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using LazyGatherer.Solver.Actions;
using LazyGatherer.Solver.Data;

namespace LazyGatherer.UI;

public sealed class RotationNode : SimpleComponentNode
{
    private const uint ContainerNodeId = 6000;
    private const uint TextMargin = 4;

    private readonly ListNode<IconNode> actionsNode;
    private readonly TextNode expectedYieldNode;

    public RotationNode(KeyValuePair<Rotation, GatheringOutcome> outcome)
    {
        Position = new Vector2(496.0f, 68f + (44.0f * outcome.Key.Context.RowId));
        IsVisible = Service.Config.Display;
        actionsNode = new IconNodeList()
        {
            Options = new List<IconNode>(),
            IsVisible = true,
            Color = KnownColor.White.Vector(),
            Margin = new Spacing(TextMargin),
        };

        Service.NativeController.AttachNode(actionsNode, this, NodePosition.AsLastChild);

        var context = outcome.Key.Context;
        var kv = this.CompactActions(outcome.Key.Actions);
        var index = 0u;
        foreach (var action in kv)
        {
            var iconNode = new IconNode(context, action.Key, action.Value, index);
            Service.NativeController.AttachNode(iconNode, this, NodePosition.AsLastChild);
            actionsNode.Options.Add(iconNode);
            index++;
        }

        expectedYieldNode = new TextNode
        {
            Position = new Vector2(TextMargin + (44 * index), 26),
            Size = new Vector2(20),
            FontType = FontType.Axis,
            TextFlags = TextFlags.Edge,
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = KnownColor.Black.Vector(),
            FontSize = 14,
            Text = $"Expected yield: {Math.Round(outcome.Value.Yield, 1)} for {outcome.Value.UsedGp} GP",
            IsVisible = true,
        };

        Service.NativeController.AttachNode(expectedYieldNode, this, NodePosition.AsLastChild);
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
            actionsNode?.Options?.ForEach(iconNode => iconNode.Dispose());
            actionsNode?.Dispose();
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
