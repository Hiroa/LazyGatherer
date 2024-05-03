using System;
using System.Collections.Generic;
using System.Drawing;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.NativeUi;
using LazyGatherer.Models;
using LazyGatherer.Solver.Actions;
using LazyGatherer.Solver.Data;

namespace LazyGatherer.UI;

public unsafe class RotationUI : IDisposable
{
    private const uint ContainerNodeId = 6000;
    private const uint IconNodeId = ContainerNodeId + 100;
    private const uint FrameNodeId = ContainerNodeId + 200;
    private const uint TextNodeId = ContainerNodeId + 300;
    private readonly AtkUnitBase* addonGathering = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering");
    private readonly ResNode rootNode;
    private readonly TextNode yieldNode;
    private readonly List<ImageNode> actionNodes = [];
    private readonly List<IDisposable> styleNodes = [];
    private readonly Rotation rotation;


    public RotationUI(KeyValuePair<Rotation, GatheringOutcome> outcome, Config config)
    {
        rotation = outcome.Key;
        var context = rotation.Context;

        // Root
        rootNode = new ResNode(new ResNodeOptions
        {
            Id = ContainerNodeId,
            Position = new Vector2(500.0f, 68f + (44.0f * context.RowId)),
            Size = new Vector2(200.0f, 48.0f),
        });
        rootNode.ResourceNode->ToggleVisibility(config.Display);
        Node.LinkNodeAtStart(rootNode.ResourceNode, addonGathering);

        // Draw action
        var kv = this.CompactActions(rotation.Actions);
        var index = 0u;
        foreach (var action in kv)
        {
            // Action Node
            this.DrawActionIcon(action, context, index);

            // Frame - for style
            this.DrawIconFrame(index);

            if (action.Value > 1)
            {
                this.DrawActionText(action, index);
            }

            index++;
        }

        yieldNode = new TextNode(new TextNodeOptions
        {
            Id = ContainerNodeId + 2,
        });
        yieldNode.ResourceNode->SetX(4f + (44 * index));
        yieldNode.ResourceNode->SetY(34f);
        yieldNode.Node->SetText($"Expected yield: {Math.Round(outcome.Value.Yield, 1)}");
        yieldNode.ResourceNode->ToggleVisibility(config.DisplayEstimatedYield);
        rootNode.AddResourceNode(yieldNode, addonGathering);
    }

    private void DrawActionIcon(KeyValuePair<BaseAction, int> action, GatheringContext context, uint index)
    {
        var actionNode = new ImageNode(new ImageNodeOptions
        {
            Id = IconNodeId + index,
            Color = KnownColor.White.Vector()
        });
        actionNode.ResourceNode->SetX(4f + (44 * index));
        actionNode.ResourceNode->SetY(4f);
        actionNode.ResourceNode->SetWidth(40);
        actionNode.ResourceNode->SetHeight(40);
        actionNode.ResourceNode->NodeFlags = NodeFlags.Visible;
        String iconTextureId;
        switch (context.Job)
        {
            case Job.Min:
                iconTextureId = action.Key.MinerAction.Icon.ToString("000000");
                // actionNode.Node->LoadIconTexture(action.Key.MinerAction.Icon, 1); // Do not use, randomly break the icons
                break;
            case Job.Bot:
                iconTextureId = action.Key.BotanistAction.Icon.ToString("000000");
                // actionNode.Node->LoadIconTexture(action.Key.BotanistAction.Icon, 1); // Do not use, randomly break the icons
                break;
            default:
                iconTextureId = "000000";
                break;
        }

        actionNode.Node->LoadTexture($"ui/icon/001000/{iconTextureId}.tex");

        actionNodes.Add(actionNode);
        rootNode.AddResourceNode(actionNode, addonGathering);
    }

    private void DrawIconFrame(uint index)
    {
        var frameNode = new ImageNode(new ImageNodeOptions
        {
            Id = FrameNodeId + index,
            Color = KnownColor.White.Vector(),
            Flags = 0
        });
        frameNode.ResourceNode->SetX(44 * index);
        frameNode.ResourceNode->SetWidth(48);
        frameNode.ResourceNode->SetHeight(48);
        frameNode.ResourceNode->NodeFlags = NodeFlags.Visible;
        frameNode.Node->LoadTexture("ui/uld/IconA_Frame.tex"); // Default work fine for all styles
        styleNodes.Add(frameNode);
        rootNode.AddResourceNode(frameNode, addonGathering);
    }

    private void DrawActionText(KeyValuePair<BaseAction, int> action, uint index)
    {
        var countNode = new TextNode(new TextNodeOptions
        {
            Id = TextNodeId + index,
            Alignment = AlignmentType.Right
        });
        countNode.ResourceNode->SetX(4f + (44 * index));
        countNode.ResourceNode->SetY(4f);
        countNode.ResourceNode->SetWidth(40);
        countNode.Node->SetText($"x{action.Value}");
        styleNodes.Add(countNode);
        rootNode.AddResourceNode(countNode, addonGathering);
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

    public void Dispose()
    {
        yieldNode.Dispose();
        styleNodes.ForEach(an => an.Dispose());
        actionNodes.ForEach(an => an.Dispose());
        rootNode.Dispose();
    }

    public void OnFramework()
    {
        if (rootNode.ResourceNode->IsVisible)
        {
            // Automatic gathering text node
            var autoGathering = addonGathering->UldManager.SearchNodeById(2);
            rootNode.ResourceNode->ToggleVisibility(!autoGathering->IsVisible);
        }
    }

    public void Update(Config config)
    {
        rootNode.ResourceNode->ToggleVisibility(config.Display);
        yieldNode.ResourceNode->ToggleVisibility(config.DisplayEstimatedYield);
    }
}
