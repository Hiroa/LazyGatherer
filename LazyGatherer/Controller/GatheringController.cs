using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using LazyGatherer.Solver;
using LazyGatherer.Solver.Comparator;
using LazyGatherer.Solver.Data;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Controller;

public class GatheringController : IDisposable
{
    // public readonly List<KeyValuePair<Rotation, GatheringOutcome>> GatheringOutcomes = [];

    private const uint BaseNodeItemId = 17;
    private readonly RotationGenerator rotationGenerator = new();
    private bool firstDraw;

    public unsafe GatheringController()
    {
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "Gathering", OnGatheringNodeEvent);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "Gathering", OnGatheringNodeEvent);

        // If Gathering addon is already opened
        var addon = (AddonGathering*)Service.GameGui.GetAddonByName("Gathering").Address;
        firstDraw = addon != null;
    }

    public void OnGatheringNodeEvent(AddonEvent ev, AddonArgs args)
    {
        switch (ev)
        {
            case AddonEvent.PostDraw when firstDraw:
                ComputeRotations();
                break;
            case AddonEvent.PreFinalize:
                firstDraw = true;
                break;
        }
    }

    public unsafe void ComputeRotations()
    {
        // init context
        var addon = (AddonGathering*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (addon == null || !addon->IsVisible) return;
        var contexts = GetGatheringContexts(addon);
        if (contexts.Count == 0) return;
        firstDraw = false;
        List<KeyValuePair<Rotation, GatheringOutcome>> gatheringOutcomes = [];
        foreach (var gatheringContext in contexts)
        {
            var bestOutcome = this.GetBestOutcome(gatheringContext);
            Service.Log.Debug(bestOutcome.Key.ToString(gatheringContext));
            gatheringOutcomes.Add(bestOutcome);
        }

        Service.UIController.DrawRotations(gatheringOutcomes);
    }

    public void Dispose()
    {
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "Gathering", OnGatheringNodeEvent);
    }

    private static unsafe List<GatheringContext> GetGatheringContexts(AddonGathering* addon)
    {
        var itemsId = addon->ItemIds;

        List<GatheringContext> contexts = [];
        // Player info
        var uiState = UIState.Instance();
        var playerGathering = uiState->PlayerState.Attributes[72];
        Service.Log.Verbose($"Player has {playerGathering} gathering");
        var player = Service.ClientState.LocalPlayer;
        var job = (Job)player!.ClassJob.Value.RowId;

        for (var i = 0; i < 8; i++) // 8 items max by Gathering point
        {
            // Ignore empty node
            var itemId = itemsId[i];
            if (itemId == 0) continue;
            Service.Log.Verbose($"Item id {itemId}");

            // Ignore unique object
            var item = Service.DataManager.GetExcelSheet<Item>().GetRow(itemId);
            Service.Log.Verbose($"Item is unique: {item.IsUnique}");
            if (item.IsUnique) continue;

            // Ignore collectable Object
            Service.Log.Verbose($"Item is collectable: {item.IsCollectable}");
            if (item.IsCollectable) continue;

            // Ignore seeds(20) and soil(21) (More consistent than checking for rare tag)
            if (item.FilterGroup is 20 or 21) continue;

            // Context info from gui
            var itemRow = addon->UldManager.SearchNodeById((uint)(BaseNodeItemId + i));

            // Ignore rare Object
            var isRare =
                itemRow->GetComponent()->UldManager.SearchNodeById(7)->IsVisible(); //Not consistent if rare and hidden
            Service.Log.Verbose($"Item is rare: {isRare}");
            if (isRare) continue; // Nothing impact the gathering outcome for rare item

            var chanceNode = itemRow->GetComponent()->UldManager.SearchNodeById(10)->GetAsAtkTextNode()->NodeText;
            var boonChanceNode = itemRow->GetComponent()->UldManager.SearchNodeById(16)->GetAsAtkTextNode()->NodeText;
            var iconNode = itemRow->GetComponent()->UldManager.SearchNodeById(31);
            var baseAmount = iconNode->GetComponent()->UldManager.SearchNodeById(7)->GetAsAtkTextNode()->NodeText;

            // Context info from data
            var gathering = GetRequiredGathering(itemId);

            // Compute bountifulBonus
            var bountifulBonus = ComputeBountifulBonus(playerGathering, gathering);

            var gatheringContext = new GatheringContext
            {
                RowId = (uint)i,
                Item = item,
                AvailableGp = (int)player.CurrentGp,
                BaseAmount = baseAmount.EqualToString("") ? 1 : baseAmount.ToInteger(),
                Chance = chanceNode.ToInteger() / 100.0,
                Attempts = addon->IntegrityLeftover->NodeText.ToInteger(),
                HasBoon = !boonChanceNode.EqualToString("-"),
                Boon = boonChanceNode.EqualToString("-") ? 0 : boonChanceNode.ToInteger() / 100.0,
                BountifulBonus = bountifulBonus,
                CharacterLevel = player.Level,
                Job = job,
                OneTurnRotation = Service.Config.OneTurnRotation
            };
            Service.Log.Verbose($"{gatheringContext}");
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

    private static ushort GetRequiredGathering(uint itemId)
    {
        var gItem = GetGatheringItemById(itemId);
        var itemLvlId = gItem.GatheringItemLevel.Value.RowId;
        var itemLevel = Service.DataManager.Excel.GetSheet<ItemLevel>().GetRow(itemLvlId);
        var gathering = itemLevel.Gathering;
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
        return Service.Config.YieldCalculator == 0
                   ? rotationOutcomes.MaxBy(kv => kv.Value, new GatheringMaxYieldComparer())
                   : rotationOutcomes.MaxBy(kv => kv.Value, new GatheringEfficiencyComparer());
    }

    private static GatheringItem GetGatheringItemById(uint id)
    {
        return Service.DataManager.GetExcelSheet<GatheringItem>().FirstOrDefault(x => x.Item.RowId == id);
    }
}
