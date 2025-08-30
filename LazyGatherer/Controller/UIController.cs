using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using LazyGatherer.Solver.Data;
using LazyGatherer.UI;

namespace LazyGatherer.Controller;

public class UIController : IDisposable
{
    private readonly List<RotationNode> rotationNodes = [];
    private CircleButtonNode? cog;
    private CircleButtonNode? eye;

    public UIController()
    {
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Gathering", OnGatheringPostSetup);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "Gathering", OnGatheringPreFinalize);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "Gathering", OnGatheringPostUpdate);
    }

    private unsafe void OnGatheringPostSetup(AddonEvent ev, AddonArgs args)
    {
        var gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (gatheringAddon == null)
            return;

        Service.NativeController.AttachNode(cog = new CircleButtonNode
        {
            Position = new Vector2(450.0f, 8.0f),
            Size = new Vector2(24f, 24f),
            Icon = ButtonIcon.GearCog,
            Tooltip = "LazyGatherer Configuration",
            IsVisible = true,
            OnClick = () => Service.ConfigAddon.Toggle(),
        }, gatheringAddon->RootNode, NodePosition.AsLastChild);


        Service.NativeController.AttachNode(eye = new CircleButtonNode
        {
            Position = new Vector2(428.0f, 8.0f),
            Size = new Vector2(24f, 24f),
            Icon = ButtonIcon.Eye,
            Tooltip = "LazyGatherer display",
            IsVisible = true,
            OnClick = () =>
            {
                Service.Config.Display = !Service.Config.Display;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.UpdateRotations();
            }
        }, gatheringAddon->RootNode, NodePosition.AsLastChild);
    }

    private void OnGatheringPreFinalize(AddonEvent ev, AddonArgs args)
    {
        if (cog != null)
        {
            Service.NativeController.DetachNode(cog);
            cog.Dispose();
            cog = null;
        }

        if (eye != null)
        {
            Service.NativeController.DetachNode(eye);
            eye.Dispose();
            eye = null;
        }

        ClearRotations();
    }

    public void Dispose()
    {
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "Gathering", OnGatheringPostSetup);
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "Gathering", OnGatheringPreFinalize);
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, "Gathering", OnGatheringPostUpdate);
        ClearRotations();
    }

    public unsafe void OnGatheringPostUpdate(AddonEvent ev, AddonArgs args)
    {
        var addonGathering = (AddonGathering*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (addonGathering == null)
        {
            return;
        }

        foreach (var rotationNode in rotationNodes)
        {
            // Hide while quick gathering
            if (rotationNode.IsVisible && addonGathering->GatherStatus == 2)
            {
                rotationNode.IsVisible = false;
            }
        }
    }

    public void UpdateRotations()
    {
        rotationNodes.ForEach(r => r.Update());
    }

    public unsafe void DrawRotations(List<KeyValuePair<Rotation, GatheringOutcome>> gatheringOutcomes)
    {
        AtkUnitBase* gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (gatheringAddon == null)
        {
            return;
        }

        // Clear existing rotations first
        ClearRotations();

        foreach (var go in gatheringOutcomes)
        {
            var rotationNode = new RotationNode(go);
            rotationNodes.Add(rotationNode);
            Service.NativeController.AttachNode(rotationNode, gatheringAddon->RootNode, NodePosition.AsLastChild);
        }
    }

    private unsafe void ClearRotations()
    {
        AtkUnitBase* gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (gatheringAddon != null)
        {
            rotationNodes.ForEach(r => Service.NativeController.DetachNode(r));
        }

        rotationNodes.ForEach(r => r.Dispose());
        rotationNodes.Clear();
    }
}
