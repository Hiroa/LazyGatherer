using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Controllers;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;
using LazyGatherer.Collectable;
using LazyGatherer.Collectable.Presets;
using LazyGatherer.Models;

namespace LazyGatherer.Controller;

public unsafe class MasterpieceController : IDisposable
{
    private readonly AddonController addonController;

    // UI Nodes
    private readonly Dictionary<Tuple<uint, uint>, AntsNode> antsNodes = [];
    private TextDropDownNode? rotationsDropdownNode;
    private CircleButtonNode? displayButtonNode;
    private CircleButtonNode? gpAlertButtonNode;

    // State Variables
    public bool IsEnabled { get; set; }
    public uint? CurrActionId;
    private Preset? selectedPreset;

    public MasterpieceController()
    {
        addonController = new AddonController
        {
            AddonName = "GatheringMasterpiece",
            OnSetup = OnGatheringAddonPostEnable,
            OnFinalize = OnGatheringAddonClose,
            OnUpdate = OnGatheringAddonUpdate,
        };
        Service.Framework.Run(() => addonController.Enable()).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        Service.Framework.Run(() => addonController.Dispose()).GetAwaiter().GetResult();
        IsEnabled = false;
    }

    public void Toggle()
    {
        if (IsEnabled) Disable();
        else Enable();
    }

    public void Disable()
    {
        if (rotationsDropdownNode != null)
        {
            rotationsDropdownNode.IsVisible = false;
        }

        foreach (var node in antsNodes)
        {
            node.Value.Timeline?.StopAnimation();
        }

        selectedPreset = null;
        CurrActionId = null;
        IsEnabled = false;
    }

    public void Enable()
    {
        if (rotationsDropdownNode != null)
        {
            rotationsDropdownNode.IsVisible = true;
            if (rotationsDropdownNode.SelectedOption != null)
                selectedPreset = BuiltIn.Presets[rotationsDropdownNode.SelectedOption];
        }

        IsEnabled = true;
    }

    private void OnGatheringAddonPostEnable(AtkUnitBase* addon)
    {
        // Init UI nodes
        var masterpieceAddon = (AddonGatheringMasterpiece*)addon;
        AttachAntsNode(masterpieceAddon->ScourDragDrop, Tuple.Create(22182u, 22186u));
        AttachAntsNode(masterpieceAddon->BrazenDragDrop, Tuple.Create(22183u, 22187u));
        AttachAntsNode(masterpieceAddon->MeticulousDragDrop, Tuple.Create(22184u, 22188u));
        AttachAntsNode(masterpieceAddon->ScrutinyDragDrop, Tuple.Create(22185u, 22189u));
        AttachAntsNode(masterpieceAddon->CollectDragDrop, Tuple.Create(240u, 815u));
        AttachAntsNode(masterpieceAddon->PrimingTouchDragDrop, Tuple.Create(34871u, 34872u));
        AttachAntsNode(masterpieceAddon->CollectorsFocusDragDrop, Tuple.Create(21205u, 21206u));

        displayButtonNode = new CircleButtonNode
        {
            Position = new Vector2(38, 645),
            Size = new Vector2(28, 28),
            Icon = CircleButtonIcon.Eye,
            TextTooltip = "[LazyGatherer] Toggle display",
            IsVisible = true,
            OnClick = () =>
            {
                Service.Config.CollectableDisplay = !Service.Config.CollectableDisplay;
                Service.Interface.SavePluginConfig(Service.Config);
                Toggle();
            }
        };
        displayButtonNode.AttachNode(masterpieceAddon->RootNode);

        gpAlertButtonNode = new CircleButtonNode
        {
            Position = new Vector2(64, 645),
            Size = new Vector2(28, 28),
            Icon = CircleButtonIcon.Volume,
            TextTooltip = "[LazyGatherer] GP alert config",
            IsVisible = true,
            OnClick = () => { Service.GpAlertAddon.Toggle(); }
        };
        gpAlertButtonNode.AttachNode(masterpieceAddon->RootNode);

        rotationsDropdownNode = new TextDropDownNode
        {
            Size = new Vector2(180.0f, 24.0f),
            Position = new Vector2(10.0f, 248.0f),
            IsVisible = true,
            TextTooltip = "[LazyGatherer] Select collectable rotation preset",
            Options = BuiltIn.Presets.Keys.ToList(),
            OnOptionSelected = index =>
            {
                selectedPreset = BuiltIn.Presets[index];
                Service.Config.CollectableLastRotation = index;
                Service.Interface.SavePluginConfig(Service.Config);
#if DEBUG
                Service.Log.Info(selectedPreset?.Export() ?? "Invalid preset selected.");
#endif
            }
        };
        rotationsDropdownNode.AttachNode(masterpieceAddon->RootNode);

        // Load last used preset or default to first
        var presetEntry =
            BuiltIn.Presets.FirstOrDefault(e => e.Key == Service.Config.CollectableLastRotation,
                                           BuiltIn.Presets.First());
        selectedPreset = presetEntry.Value;
        rotationsDropdownNode.SelectedOption = presetEntry.Key;

        // Set state based on config
        if (!Service.Config.CollectableDisplay)
        {
            Disable();
        }

        IsEnabled = Service.Config.CollectableDisplay;
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
        antsNode.AttachNode(frameContainer);
        antsNodes.Add(actionIds, antsNode);
    }

    public void OnGatheringAddonUpdate(AtkUnitBase* addon)
    {
        var masterpieceAddon = (AddonGatheringMasterpiece*)addon;
        if (!IsAddonReady(masterpieceAddon)
            || selectedPreset == null)
            return;
        var gatheringContexts = GetGatheringContexts(masterpieceAddon, int.MaxValue);
        var rotation = Rotation.FromPreset(selectedPreset, gatheringContexts);
        if (!rotation.IsValidRotation())
        {
            Service.Log.Info("Invalid rotation generated.");
            return;
        }

        var action = rotation.GetNextAction();

        // Only update if action changed
        if (action != null && action.GetJobAction() != CurrActionId)
        {
            CurrActionId = action.GetJobAction();
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
        // selectedPreset = null;
        if (rotationsDropdownNode != null)
        {
            rotationsDropdownNode.Dispose();
            rotationsDropdownNode = null;
        }

        if (displayButtonNode != null)
        {
            displayButtonNode.Dispose();
            displayButtonNode = null;
        }

        if (gpAlertButtonNode != null)
        {
            gpAlertButtonNode.Dispose();
            gpAlertButtonNode = null;
        }

        foreach (var keyValuePair in antsNodes)
        {
            keyValuePair.Value.Dispose();
            antsNodes.Remove(keyValuePair.Key);
        }
    }

    private static Context GetGatheringContexts(AddonGatheringMasterpiece* addon, int maxGpToUse)
    {
        // Player info
        var player = Service.ObjectTable.LocalPlayer;
        var gpToUse = Math.Min((int)player!.CurrentGp, maxGpToUse);
        var job = (Job)player.ClassJob.Value.RowId;

        // Check player status
        var scrutinyUsed = player.StatusList.Any(s => s.StatusId == 757);
        var collectorStandard = player.StatusList.Any(s => s.StatusId == 2418);
        var collectorHighStandard = player.StatusList.Any(s => s.StatusId == 3911);
        var eureka = player.StatusList.Any(s => s.StatusId == 2765);
        var priming = player.StatusList.Any(s => s.StatusId == 3910);
        var focus = player.StatusList.Any(s => s.StatusId == 2668);

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
            HasEureka = eureka,
            HasPriming = priming,
            HasFocus = focus
        };

        return context;
    }

    private static bool IsAddonReady(AddonGatheringMasterpiece* addon)
    {
        return addon->ItemName != null && !string.IsNullOrEmpty(addon->ItemName->NodeText.ToString());
    }
}
