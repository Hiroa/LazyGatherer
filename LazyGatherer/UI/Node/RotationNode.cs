using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using LazyGatherer.Gathering.Actions;
using LazyGatherer.Gathering.Models;
using LazyGatherer.Models;

namespace LazyGatherer.UI.Node;

public sealed class RotationNode : SimpleComponentNode
{
    private readonly TextNode? estimatedYieldNode;
    private readonly SimpleNineGridNode? backgroundNode;
    private readonly int actionNodeCount;

    public override Vector2 Size
    {
        get;
        set
        {
            field = value;
            UpdateBackgroundSize(value);
        }
    }

    public RotationNode(KeyValuePair<Rotation, GatheringOutcome> outcome)
    {
        var gatheringContext = outcome.Key.Context;
        Position = new Vector2(501.0f, 71f + (44.0f * gatheringContext.RowId));
        IsVisible = Service.Config.Display;

        backgroundNode = new SimpleNineGridNode
        {
            Position = new Vector2(-5f, -5f),
            Size = new Vector2(496f, 58f),
            TexturePath = "ui/uld/Gathering_hr1.tex",
            TextureCoordinates = new Vector2(72, 84),
            TextureSize = new Vector2(24, 24),
            Offsets = new Vector4(10, 10, 10, 10),
            IsVisible = Service.Config.DisplayBackground,
            Alpha = 0.8f
        };
        backgroundNode.AttachNode(this);

        foreach (var (baseAction, count) in CompactActions(outcome.Key.Actions))
        {
            new ActionNode
            {
                IsVisible = true,
                Position = new Vector2(39 * actionNodeCount, 0),
                Size = new Vector2(48, 48),
                Count = count,
                ActionId = GetGathererAction(baseAction, gatheringContext),
                Scale = new Vector2(0.9f, 0.9f),
            }.AttachNode(this);
            actionNodeCount++;
        }

        estimatedYieldNode = new TextNode
        {
            Position = new Vector2(4f + (39 * actionNodeCount), 23),
            TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
            TextColor = ColorHelper.GetColor(2),
            FontSize = 14,
            String = FormatEstimatedYield(outcome.Value),
            IsVisible = Service.Config.DisplayEstimatedYield,
        };
        estimatedYieldNode.AttachNode(this);


        Size = ComputeSize();
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
        estimatedYieldNode?.IsVisible = config.DisplayEstimatedYield;
        backgroundNode?.IsVisible = config.DisplayBackground;
        Size = ComputeSize();
    }

    private static string FormatEstimatedYield(GatheringOutcome outcome)
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

    private static uint GetGathererAction(BaseAction action, GatheringContext context) => context.Job switch
    {
        Job.Min => action.MinerAction.RowId,
        Job.Bot => action.BotanistAction.RowId,
        _ => 0u
    };

    private void UpdateBackgroundSize(Vector2 size)
    {
        backgroundNode?.Size = size + new Vector2(20f, 10f); // Account for background padding
    }

    private Vector2 ComputeSize()
    {
        var size = new Vector2(39 * actionNodeCount, 40f);
        return estimatedYieldNode is { IsVisible: true }
                   ? size + new Vector2(4f, 0f) + estimatedYieldNode.GetTextDrawSize(false) with { Y = 0 }
                   : size - new Vector2(5f, 0f);
    }
}
