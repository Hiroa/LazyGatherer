using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Nodes;
using LazyGatherer.Models;
using LazyGatherer.Solver.Collectable;
using LazyGatherer.Solver.Collectable.Model;

namespace LazyGatherer.Controller;

public unsafe class MasterpieceController : IDisposable
{
    private readonly AddonController addonController = new("GatheringMasterpiece");
    public uint? CurrActionId;
    private readonly Dictionary<Tuple<uint, uint>, AntsNode> antsNodes = [];

    public MasterpieceController()
    {
        addonController.OnDetach += OnGatheringAddonClose;
        addonController.OnAttach += OnGatheringAddonPostEnable;
        addonController.OnUpdate += OnGatheringAddonUpdate;
        addonController.Enable();
    }

    private void OnGatheringAddonPostEnable(AtkUnitBase* addon)
    {
        var masterpieceAddon = (AddonGatheringMasterpiece*)addon;
        AttachAntsNode(masterpieceAddon->ScourDragDrop, Tuple.Create(22182u, 22186u));
        AttachAntsNode(masterpieceAddon->BrazenDragDrop, Tuple.Create(22183u, 22187u));
        AttachAntsNode(masterpieceAddon->MeticulousDragDrop, Tuple.Create(22184u, 22188u));
        AttachAntsNode(masterpieceAddon->ScrutinyDragDrop, Tuple.Create(22185u, 22189u));
        AttachAntsNode(masterpieceAddon->CollectDragDrop, Tuple.Create(240u, 815u));
        // AttachAntsNode(masterpieceAddon->BrazenDragDrop, Tuple.Create(22183u, 22187u));
        // AttachAntsNode(masterpieceAddon->BrazenDragDrop, Tuple.Create(22183u, 22187u));
    }

    private void AttachAntsNode(AtkComponentDragDrop* dragDrop, Tuple<uint, uint> actionIds)
    {
        if (dragDrop == null)
            return;
        var frameContainer = dragDrop->AtkComponentIcon->FrameContainer;
        var antsNode = new AntsNode
        {
            Size = new Vector2(48.0f, 48.0f),
            IsVisible = false
        };
        antsNode.AntsImageNode.IsVisible = true;
        antsNode.AddTimeline(new TimelineBuilder()
                             .BeginFrameSet(1, 9)
                             .AddLabel(1, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
                             .AddLabel(2, 26, AtkTimelineJumpBehavior.Start, 0)
                             .AddLabel(9, 0, AtkTimelineJumpBehavior.LoopForever, 26)
                             .EndFrameSet()
                             .Build()
        );
        Service.NativeController.AttachNode(antsNode, frameContainer);
        antsNodes.Add(actionIds, antsNode);
    }

    public void OnGatheringAddonUpdate(AtkUnitBase* addon)
    {
        var masterpieceAddon = (AddonGatheringMasterpiece*)addon;
        if (!IsAddonReady(masterpieceAddon))
            return;
        var gatheringContexts = GetGatheringContexts(masterpieceAddon, int.MaxValue);
        var import =
            "H4sIAAAAAAAACtWTQUvEMBCF/0qZcw7xsCC5rYush9UKu3iRHkI7uwbSTJwkii7979JdS6FUKQXBPQ5k3nsf83KEB10jKLiSUmZrnzFFHQ25bM9UZ7sVCLg3boNvaEEt5Glae1DtewG7xC6Aej7Cilxl2sVurL1mE8jlHllHYlBwZw4vyDnfviZtQfQ7uQMFj0wHxhAMORDwpG3Cs01TCFiWnTYsY8Tax9O+tVhGKBoxLcCG3n/273U774HxPLtpnNdSnlVHZMbjfqch3kbtKs1VL7bXNuAg/LbkFI37aO+J0ZTJUgr/HyVy+oXkhvUnuukUMwr41zRzjjEHYzH8RpfUgvFCU2IomqL5AnacfTTEBAAA";
        var rotation = RotationManager.Import(import, gatheringContexts);
        if (!RotationManager.IsValidRotation(rotation))
        {
            Service.Log.Info("Invalid rotation generated.");
            return;
        }

        var action = RotationManager.GetNextAction(rotation);

        if (action != null && action.GetJobAction() != CurrActionId)
        {
            CurrActionId = action.GetJobAction();
            Service.Log.Info($"Next action: {CurrActionId}");
            foreach (var (key, antsNode) in antsNodes)
            {
                if (key.Item1 == CurrActionId || key.Item2 == CurrActionId)
                {
                    antsNode.IsVisible = true;
                    antsNode.Timeline?.PlayAnimation(AtkTimelineJumpBehavior.LoopForever, 26);
                }
                else
                {
                    antsNode.IsVisible = false;
                    antsNode.Timeline?.StopAnimation();
                }
            }
        }
    }

    private void OnGatheringAddonClose(AtkUnitBase* _)
    {
        CurrActionId = null;
        foreach (var keyValuePair in antsNodes)
        {
            Service.NativeController.DetachNode(keyValuePair.Value);
            antsNodes.Remove(keyValuePair.Key);
        }
    }

    public void Dispose()
    {
        addonController.Dispose();
    }

    private static Context GetGatheringContexts(AddonGatheringMasterpiece* addon, int maxGpToUse)
    {
        // Player info
        var player = Service.ClientState.LocalPlayer;
        var gpToUse = Math.Min((int)player!.CurrentGp, maxGpToUse);
        var job = (Job)player.ClassJob.Value.RowId;

        // Check player status
        var scrutinyUsed = player.StatusList.Any(s => s.StatusId == 757);
        var collectorStandard = player.StatusList.Any(s => s.StatusId == 2418);
        var collectorHighStandard = player.StatusList.Any(s => s.StatusId == 3911);
        var eureka = player.StatusList.Any(s => s.StatusId == 2765);

        // Get addon info
        var progression = addon->GetTextNodeById(6)->NodeText.ToInteger();

        var context = new Context
        {
            CharacterLevel = player.Level,
            Job = job,
            AvailableGp = gpToUse,
            ItemName = addon->ItemName->NodeText.ToString(),
            Progression = progression,
            Chance = addon->ObtainChance->NodeText.ToInteger(),
            Attempts = addon->IntegrityLeftover->NodeText.ToInteger(),
            MaxAttempts = addon->IntegrityTotal->NodeText.ToInteger(),
            HasCollectorStandard = collectorStandard || collectorHighStandard,
            HasScrutiny = scrutinyUsed,
            HasEureka = eureka
        };

        return context;
    }

    private static bool IsAddonReady(AddonGatheringMasterpiece* addon)
    {
        return addon->ItemName != null && !string.IsNullOrEmpty(addon->ItemName->NodeText.ToString());
    }
}
