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
    private readonly TextNode? expectedYieldNode;

    public RotationNode(KeyValuePair<Rotation, GatheringOutcome> outcome)
    {
        var gatheringContext = outcome.Key.Context;
        Position = new Vector2(496.0f, 68f + (44.0f * gatheringContext.RowId));
        IsVisible = Service.Config.Display;

        var index = 0u;
        foreach (var (baseAction, count) in CompactActions(outcome.Key.Actions))
        {
            AttachNode(new ActionNode(gatheringContext, baseAction, count)
            {
                IsVisible = true,
                Position = new Vector2(44 * index, 0),
                Size = new Vector2(48, 48),
            });
            index++;
        }

        AttachNode(expectedYieldNode = new TextNode
        {
            Position = new Vector2(4f + (44 * index), 26),
            TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
            FontSize = 14,
            SeString = $"Expected yield: {Math.Round(outcome.Value.Yield, 1)} for {outcome.Value.UsedGp} GP",
            IsVisible = true,
        });
    }

    /**
     * Compacts a list of actions into a dictionary with action counts.
     */
    private static Dictionary<BaseAction, int> CompactActions(List<BaseAction> actions)
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

    private void AttachNode(NodeBase node)
    {
        Service.NativeController.AttachNode(node, this, NodePosition.AsLastChild);
    }

    public void Update()
    {
        var config = Service.Config;
        IsVisible = config.Display;
        if (expectedYieldNode != null)
            expectedYieldNode.IsVisible = config.DisplayEstimatedYield;
    }
}
