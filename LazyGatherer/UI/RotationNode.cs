using System;
using System.Collections.Generic;
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
    private readonly List<NodeBase> actionsNode = new();
    private readonly TextNode expectedYieldNode;

    public RotationNode(KeyValuePair<Rotation, GatheringOutcome> outcome)
    {
        var context = outcome.Key.Context;
        Position = new Vector2(496.0f, 68f + (44.0f * context.RowId));
        IsVisible = Service.Config.Display;

        var kv = this.CompactActions(outcome.Key.Actions);
        var index = 0u;
        foreach (var (baseAction, count) in kv)
        {
            var actionNode = new ActionNode(context, baseAction, count, index);
            Service.NativeController.AttachNode(actionNode, this, NodePosition.AsLastChild);
            actionsNode.Add(actionNode);
            index++;
        }

        expectedYieldNode = new TextNode
        {
            Position = new Vector2(4f + (44 * index), 26),
            TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
            FontSize = 14,
            Text = $"Expected yield: {Math.Round(outcome.Value.Yield, 1)} for {outcome.Value.UsedGp} GP",
            IsVisible = true,
        };
        Service.NativeController.AttachNode(expectedYieldNode, this, NodePosition.AsLastChild);

        Size = ComputeSize();
    }

    private Vector2 ComputeSize()
    {
        var width = 44 * actionsNode.Count + 4 + expectedYieldNode.Size.X;
        return new Vector2(width, 48);
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
        try
        {
            if (disposing)
            {
                actionsNode.ForEach(iconNode => iconNode.Dispose());
                expectedYieldNode.Dispose();
                base.Dispose(disposing);
            }
        }
        catch (Exception e)
        {
            Service.Log.Error("Error disposing RotationNode: " + e);
        }
    }

    public void Update()
    {
        var config = Service.Config;
        IsVisible = config.Display;
        expectedYieldNode.IsVisible = config.DisplayEstimatedYield;
    }
}
