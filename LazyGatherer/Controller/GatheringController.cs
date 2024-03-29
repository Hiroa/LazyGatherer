using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using LazyGatherer.Solver;
using LazyGatherer.Solver.Comparator;
using LazyGatherer.Solver.Data;
using Lumina.Excel.GeneratedSheets2;

namespace LazyGatherer.Controller;

public class GatheringController : IDisposable
{
    public readonly List<KeyValuePair<Rotation, GatheringOutcome>> GatheringOutcomes = [];

    private const uint BaseNodeItemId = 6;
    private readonly RotationGenerator rotationGenerator = new();

    public unsafe void OnFrameworkUpdate()
    {
        // Check Gathering addon is opened
        var addon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering");
        if (addon is null || !IsGatheringPointLoaded((AddonGathering*)addon))
        {
            if (GatheringOutcomes.Count != 0)
                GatheringOutcomes.Clear();
            return;
        }

        // Check context isn't already loaded
        if (GatheringOutcomes.Count != 0)
        {
            return;
        }

        // init context
        var contexts = GetGatheringContexts(addon);
        foreach (var gatheringContext in contexts)
        {
            GatheringOutcomes.Add(this.GetBestOutcome(gatheringContext));
        }
    }

    public void Dispose() { }

    private static unsafe List<GatheringContext> GetGatheringContexts(AtkUnitBase* addon)
    {
        var itemsId = GetItemsIdList((AddonGathering*)addon);
        List<GatheringContext> contexts = [];
        // Player info
        var uiState = UIState.Instance();
        var playerGathering = uiState->PlayerState.Attributes[72];

        // Gathering point
        var player = Service.ClientState.LocalPlayer;
        var job = (Job)player!.ClassJob.Id;
        var gpId = player.TargetObject!.DataId;
        var gp = Service.DataManager.Excel.GetSheet<GatheringPoint>()!.GetRow(gpId)!;

        for (var i = 0; i < 8; i++) // 8 items max by Gathering point
        {
            // Ignore empty node
            var itemId = itemsId[i];
            if (itemId == 0) continue;

            // Ignore unique object
            var item = Service.DataManager.GetExcelSheet<Item>()!.GetRow(itemId);
            if (item!.IsUnique) continue;

            // Context info from gui
            var itemRow = addon->UldManager.SearchNodeById((uint)(BaseNodeItemId + i));
            
            // Ignore rare Object
            var isRare = itemRow->GetComponent()->UldManager.SearchNodeById(7)->IsVisible;       
            if (isRare) continue;
            
            var chanceNode = itemRow->GetComponent()->UldManager.SearchNodeById(10)->GetAsAtkTextNode()->NodeText;
            var boonChanceNode = itemRow->GetComponent()->UldManager.SearchNodeById(16)->GetAsAtkTextNode()->NodeText;
            var iconNode = itemRow->GetComponent()->UldManager.SearchNodeById(31);
            var baseAmount = iconNode->GetComponent()->UldManager.SearchNodeById(4)->GetAsAtkTextNode()->NodeText;

            // Context info from data
            var gathering = GetRequiredGathering(itemId);

            // Compute bountifulBonus
            var bountifulBonus = ComputeBountifulBonus(playerGathering, gathering);
            var gatheringContext = new GatheringContext
            {
                RowId = (uint)i,
                Item = item,
                AvailableGp = (int)player.CurrentGp,
                BaseAmount = baseAmount.EqualsString("") ? 1 : baseAmount.ToInteger(),
                Chance = chanceNode.ToInteger() / 100.0,
                Attempts = gp.Count,
                HasBoon = !boonChanceNode.EqualsString("-"),
                Boon = boonChanceNode.EqualsString("-") ? 0 : boonChanceNode.ToInteger() / 100.0,
                BountifulBonus = bountifulBonus,
                CharacterLevel = player.Level,
                Job = job
            };
            Service.Log.Debug($"{gatheringContext}");
            contexts.Add(gatheringContext);
        }

        return contexts;
    }

    private static int ComputeBountifulBonus(int playerGathering, ushort gathering)
    {
        if (playerGathering > gathering * 1.1)
        {
            return 3;
        }

        return playerGathering > gathering * 0.9 ? 2 : 1;
    }

    // Return the required gathering stat 
    private static ushort GetRequiredGathering(uint itemId)
    {
        var gItem = GetGatheringItemById(itemId);
        var gatheringLevel = gItem!.GatheringItemLevel!.Value!.GatheringItemLevel;
        var itemLevel = Service.DataManager.Excel.GetSheet<ItemLevel>()!.GetRow(gatheringLevel);
        var gathering = itemLevel!.Gathering;
        return gathering;
    }

    private KeyValuePair<Rotation, GatheringOutcome> GetBestOutcome(GatheringContext context)
    {
        var rotations = rotationGenerator.GetRotations(context);
        var baseOutcome = GatheringCalculator.CalculateOutcome(rotations[0]);
        var rotationOutcomes = new Dictionary<Rotation, GatheringOutcome>();
        rotations.ForEach(r =>
        {
            var outcome = GatheringCalculator.CalculateOutcome(r, baseOutcome);
            rotationOutcomes[r] = outcome;
        });
        return rotationOutcomes.MaxBy(kv => kv.Value, new GatheringYieldComparer());
    }

    private static unsafe bool IsGatheringPointLoaded(AddonGathering* addon)
    {
        //Check if any item is loaded
        return addon->GatheredItemId1 != 0
               || addon->GatheredItemId2 != 0
               || addon->GatheredItemId3 != 0
               || addon->GatheredItemId4 != 0
               || addon->GatheredItemId5 != 0
               || addon->GatheredItemId6 != 0
               || addon->GatheredItemId7 != 0
               || addon->GatheredItemId8 != 0;
    }

    private static unsafe List<uint> GetItemsIdList(AddonGathering* addon)
    {
        return
        [
            addon->GatheredItemId1,
            addon->GatheredItemId2,
            addon->GatheredItemId3,
            addon->GatheredItemId4,
            addon->GatheredItemId5,
            addon->GatheredItemId6,
            addon->GatheredItemId7,
            addon->GatheredItemId8
        ];
    }

    private static GatheringItem? GetGatheringItemById(uint id)
    {
        return Service.DataManager.GetExcelSheet<GatheringItem>()!.FirstOrDefault(x => x.Item.Row == id);
    }
}
