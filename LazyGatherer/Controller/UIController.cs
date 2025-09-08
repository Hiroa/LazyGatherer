using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using LazyGatherer.Solver.Models;
using LazyGatherer.UI;

namespace LazyGatherer.Controller;

public class UIController : IDisposable
{
    private readonly List<RotationNode> rotationNodes = [];
    private CircleButtonNode? configButtonNode;
    private CircleButtonNode? displayButtonNode;
    private GpSliderNode? sliderNode;

    private bool uiInitialized;

    public UIController()
    {
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Gathering", OnGatheringPostSetup);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "Gathering", OnGatheringPreFinalize);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "Gathering", OnGatheringPostUpdate);
    }

    public void Dispose()
    {
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "Gathering", OnGatheringPostSetup);
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "Gathering", OnGatheringPreFinalize);
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, "Gathering", OnGatheringPostUpdate);
        ClearUI();
    }

    private void OnGatheringPostSetup(AddonEvent ev, AddonArgs args) => InitUI();
    private void OnGatheringPreFinalize(AddonEvent ev, AddonArgs args) => ClearUI();

    public unsafe void OnGatheringPostUpdate(AddonEvent ev, AddonArgs args)
    {
        var addonGathering = (AddonGathering*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (addonGathering == null)
        {
            return;
        }

        // If plugin is load during gathering
        if (!uiInitialized)
        {
            InitUI();
        }

        // Hide while quick gathering
        if (rotationNodes.Any(n => n.IsVisible) && addonGathering->GatherStatus == 2)
        {
            if (displayButtonNode is { IsVisible: true }) displayButtonNode.IsVisible = false;
            if (configButtonNode is { IsVisible: true }) configButtonNode.IsVisible = false;
            rotationNodes.ForEach(r => r.IsVisible = false);
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

    private unsafe void InitUI()
    {
        var gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (gatheringAddon == null)
            return;

        Service.NativeController.AttachNode(configButtonNode = new CircleButtonNode
        {
            Position = new Vector2(450.0f, 8.0f),
            Size = new Vector2(24f, 24f),
            Icon = ButtonIcon.GearCog,
            Tooltip = "LazyGatherer Configuration",
            IsVisible = true,
            OnClick = () => Service.ConfigAddon.Toggle(),
        }, gatheringAddon->RootNode, NodePosition.AsLastChild);


        Service.NativeController.AttachNode(displayButtonNode = new CircleButtonNode
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

        Service.NativeController.AttachNode(sliderNode = new GpSliderNode(1000)
        {
            Position = new Vector2(320, 460),
            Size = new Vector2(200, 28),
            IsVisible = true,
        }, gatheringAddon->RootNode, NodePosition.AsLastChild);
        uiInitialized = true;
    }

    private void ClearUI()
    {
        if (configButtonNode != null)
        {
            Service.NativeController.DetachNode(configButtonNode);
            configButtonNode.Dispose();
            configButtonNode = null;
        }

        if (displayButtonNode != null)
        {
            Service.NativeController.DetachNode(displayButtonNode);
            displayButtonNode.Dispose();
            displayButtonNode = null;
        }

        if (sliderNode != null)
        {
            Service.NativeController.DetachNode(sliderNode);
            sliderNode.Dispose();
            sliderNode = null;
        }

        ClearRotations();
        uiInitialized = false;
    }
}
