using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using LazyGatherer.Models;
using LazyGatherer.Solver.Gathering.Actions;
using LazyGatherer.Solver.Gathering.Models;

namespace LazyGatherer.UI;

public sealed class RotationNode : CustomNode
{
    private readonly TextNode? estimatedYieldNode;

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

        AttachNode(estimatedYieldNode = new TextNode
        {
            Position = new Vector2(4f + (44 * index), 26),
            TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
            FontSize = 14,
            SeString = FormatEstimatedYield(outcome.Value),
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

    public void Update()
    {
        var config = Service.Config;
        IsVisible = config.Display;
        if (estimatedYieldNode != null)
            estimatedYieldNode.IsVisible = config.DisplayEstimatedYield;
    }

    private static String FormatEstimatedYield(GatheringOutcome outcome)
    {
        var roundedYield = Math.Round(outcome.Yield, 1);
        var roundedMinYield = Math.Round(outcome.MinYield, 1);
        var roundedMaxYield = Math.Round(outcome.MaxYield, 1);
        return Service.Config.EstimatedYieldStyle switch
        {
            EstimatedYieldStyle.Short => $"{roundedYield}",
            EstimatedYieldStyle.ShortWithMinMax => $"{roundedYield} ({roundedMinYield}/{roundedMaxYield})",
            EstimatedYieldStyle.Basic => $"Estimated yield: {roundedYield}",
            EstimatedYieldStyle.BasicWithMinMax =>
                $"Estimated yield: {roundedYield} ({roundedMinYield}/{roundedMaxYield})",
            EstimatedYieldStyle.Detailed => $"Estimated yield: {roundedYield} for {outcome.UsedGp} GP",
            EstimatedYieldStyle.DetailedWithMinMax =>
                $"Estimated yield: {roundedYield} ({roundedMinYield}/{roundedMaxYield}) for {outcome.UsedGp} GP",
            _ => $"Estimated yield: {roundedYield}"
        };
    }
}
