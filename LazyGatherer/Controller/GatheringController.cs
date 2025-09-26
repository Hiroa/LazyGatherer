using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using LazyGatherer.Components;
using LazyGatherer.Models;
using LazyGatherer.Solver;
using LazyGatherer.Solver.Comparator;
using LazyGatherer.Solver.Models;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Controller;

public class GatheringController : IDisposable
{
    private readonly AddonController addonController = new("Gathering");
    private readonly RotationGenerator rotationGenerator = new();
    private bool rotationAlreadyComputed;

    private readonly List<RotationComparer> rotationComparers =
        [new GatheringMaxYieldComparer(), new GatheringEfficiencyComparer()];

    public unsafe GatheringController()
    {
        addonController.OnDetach += OnGatheringAddonClose;
        addonController.OnUpdate += OnGatheringAddonUpdate;
        addonController.Enable();
    }

    public unsafe void OnGatheringAddonUpdate(AtkUnitBase* addon)
    {
        if (!rotationAlreadyComputed)
        {
            ComputeRotations();
        }
    }

    private unsafe void OnGatheringAddonClose(AtkUnitBase* _) => rotationAlreadyComputed = false;


    public unsafe void ComputeRotations(int maxGpToUse = int.MaxValue)
    {
        // Check if addon is fully loaded
        var addon = (AddonGathering*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (!IsGatheringAddonLoaded(addon)) return;

        // Get context for each item
        var contexts = GetGatheringContexts(addon, maxGpToUse);
        if (contexts.Count == 0) return;

        // Compute the best rotation for each item
        List<KeyValuePair<Rotation, GatheringOutcome>> gatheringOutcomes = [];
        foreach (var gatheringContext in contexts)
        {
            var bestOutcome = this.GetBestOutcome(gatheringContext);
            Service.Log.Debug(bestOutcome.Key.ToString(gatheringContext));
            gatheringOutcomes.Add(bestOutcome);
        }

        // Update UI
        Service.UIController.DrawRotations(gatheringOutcomes);
        rotationAlreadyComputed = true;
    }

    public void Dispose()
    {
        addonController.Dispose();
    }

    private static unsafe List<GatheringContext> GetGatheringContexts(AddonGathering* addon, int maxGpToUse)
    {
        var itemsId = addon->ItemIds;

        List<GatheringContext> contexts = [];
        // Player info
        var uiState = UIState.Instance();
        var playerGathering = uiState->PlayerState.Attributes[72];
        var player = Service.ClientState.LocalPlayer;
        var gpToUse = Math.Min((int)player!.CurrentGp, maxGpToUse);
        var job = (Job)player.ClassJob.Value.RowId;

        // Check player status
        var tidingsUsed = player.StatusList.Any(s => s.StatusId == (uint)GathererStatus.Tidings);
        var yieldUsed = player.StatusList.Any(s => s.StatusId == (uint)GathererStatus.Yield);
        var gift1Used = player.StatusList.Any(s => s.StatusId == (uint)GathererStatus.Gift1);
        var gift2Used = player.StatusList.Any(s => s.StatusId == (uint)GathererStatus.Gift2);
        var bountifulUsed = player.StatusList.Any(s => s.StatusId == (uint)GathererStatus.Bountiful);

        for (var i = 0; i < 8; i++) // 8 items max by Gathering point
        {
            // Ignore empty node
            var (itemId, itemKind) = ItemUtil.GetBaseId(itemsId[i]);
            if (itemId == 0) continue;

            var item = itemKind switch
            {
                // Item mainly from levequests
                ItemKind.EventItem =>
                    ItemBase.FromLumina(Service.DataManager.GetExcelSheet<EventItem>().GetRow(itemId)),
                // Normal, hq and collectible are in Item sheet
                _ => ItemBase.FromLumina(Service.DataManager.GetExcelSheet<Item>().GetRow(itemId)),
            };

            // Ignore unique, collectable, seeds(20) and soil(21) (More consistent than checking for rare tag)
            if (item.IsUnique ||
                item.IsCollectable ||
                item.FilterGroupId is 20 or 21) continue;

            // Context info from gui
            var itemComponent = new GatheringItemComponent(addon, i);

            // Ignore rare Object
            // Not consistent if rare and hidden
            if (itemComponent.IsRare) continue; // Nothing impact the gathering outcome for rare item

            // Context info from data
            var gatheringRequired = GetRequiredGathering(itemId);

            // Compute bountifulBonus
            var bountifulBonus = ComputeBountifulBonus(playerGathering, gatheringRequired);

            var gatheringContext = new GatheringContext
            {
                RowId = (uint)i,
                Item = item,
                AvailableGp = gpToUse,
                BaseAmount = bountifulUsed ? itemComponent.BaseAmount - bountifulBonus : itemComponent.BaseAmount,
                Chance = itemComponent.GatheringChance / 100.0,
                Attempts = addon->IntegrityLeftover->NodeText.ToInteger(),
                HasBoon = itemComponent.HasBoon,
                Boon = itemComponent.BoonChance / 100.0,
                BoonBonus = tidingsUsed ? 2 : 1, // +1 if tidings already used
                BountifulBonus = bountifulBonus,
                BountifulAttempts = bountifulUsed ? 1 : 0,
                CharacterLevel = player.Level,
                Job = job,
                OneTurnRotation = Service.Config.OneTurnRotation,
                TidingsUsed = tidingsUsed,
                YieldUsed = yieldUsed,
                Gift1Used = gift1Used,
                Gift2Used = gift2Used
            };
            Service.Log.Verbose($"{gatheringContext}");
            contexts.Add(gatheringContext);
        }

        return contexts;
    }

    private static unsafe bool IsGatheringAddonLoaded(AddonGathering* addon)
    {
        return addon != null
               && addon->IsVisible
               && addon->ItemIds.ToArray().ToList().Any(id => id != 0);
    }

    private static int ComputeBountifulBonus(int playerGathering, ushort gatheringRequired)
    {
        return playerGathering switch
        {
            _ when playerGathering > gatheringRequired * 1.1 => 3,
            _ when playerGathering > gatheringRequired * 0.9 => 2,
            _ => 1
        };
    }

    private static ushort GetRequiredGathering(uint itemId)
    {
        var gItem = GetGatheringItemById(itemId);
        var itemLvlId = gItem.GatheringItemLevel.Value.RowId;
        var itemLevel = Service.DataManager.Excel.GetSheet<ItemLevel>().GetRow(itemLvlId);
        return itemLevel.Gathering;
    }

    private KeyValuePair<Rotation, GatheringOutcome> GetBestOutcome(GatheringContext context)
    {
        var rotations = rotationGenerator.GetRotations(context);
        var baseOutcome = GatheringCalculator.CalculateOutcome(rotations[0]);
        var rotationOutcomes = new Dictionary<Rotation, GatheringOutcome>();
        rotations.ForEach(r => { rotationOutcomes[r] = GatheringCalculator.CalculateOutcome(r, baseOutcome); });

        var rotationComparer = rotationComparers.First(it => it.Name == Service.Config.RotationCalculator);
        return rotationOutcomes.MaxBy(kv => kv.Value, rotationComparer);
    }

    private static GatheringItem GetGatheringItemById(uint id)
    {
        return Service.DataManager.GetExcelSheet<GatheringItem>().FirstOrDefault(x => x.Item.RowId == id);
    }
}
