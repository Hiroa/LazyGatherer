using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using LazyGatherer.Solver.Models;
using LazyGatherer.UI;

namespace LazyGatherer.Controller;

public class UIController : IDisposable
{
    private readonly AddonController addonController = new("Gathering");
    private readonly List<RotationNode> rotationNodes = [];
    private CircleButtonNode? configButtonNode;
    private CircleButtonNode? displayButtonNode;
    private GpSliderNode? sliderNode;

    public unsafe UIController()
    {
        addonController.OnAttach += OnGatheringPostSetup;
        addonController.OnDetach += OnGatheringPreFinalize;
        addonController.OnUpdate += OnGatheringPostUpdate;
        addonController.Enable();
    }

    public void Dispose()
    {
        addonController.Dispose();
    }

    private unsafe void OnGatheringPostSetup(AtkUnitBase* addon) => InitUI(addon);

    private unsafe void OnGatheringPreFinalize(AtkUnitBase* addon) => ClearUI(addon);

    public unsafe void OnGatheringPostUpdate(AtkUnitBase* addon)
    {
        var addonGathering = (AddonGathering*)addon;
        if (addonGathering == null)
        {
            return;
        }

        // Hide while quick gathering
        if (rotationNodes.Any(n => n.IsVisible) && addonGathering->GatherStatus == 2)
        {
            if (displayButtonNode is { IsVisible: true }) displayButtonNode.IsVisible = false;
            if (configButtonNode is { IsVisible: true }) configButtonNode.IsVisible = false;
            if (sliderNode is { IsVisible: true }) sliderNode.IsVisible = false;
            rotationNodes.ForEach(r => r.IsVisible = false);
        }
    }

    public void Update()
    {
        if (sliderNode != null)
        {
            sliderNode.IsVisible = Service.Config.Display && Service.Config.DisplayGpSlider;
        }

        rotationNodes.ForEach(r => r.Update());
    }

    public unsafe void DrawRotations(List<KeyValuePair<Rotation, GatheringOutcome>> gatheringOutcomes)
    {
        var gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (gatheringAddon == null)
        {
            return;
        }

        // Clear existing rotations first
        ClearRotations(gatheringAddon);

        foreach (var go in gatheringOutcomes)
        {
            var rotationNode = new RotationNode(go);
            rotationNodes.Add(rotationNode);
            Service.NativeController.AttachNode(rotationNode, gatheringAddon->RootNode, NodePosition.AsLastChild);
        }
    }

    private unsafe void ClearRotations(AtkUnitBase* gatheringAddon)
    {
        if (gatheringAddon != null)
        {
            rotationNodes.ForEach(r => Service.NativeController.DetachNode(r));
        }

        rotationNodes.ForEach(r => r.Dispose());
        rotationNodes.Clear();
    }

    private unsafe void InitUI(AtkUnitBase* gatheringAddon)
    {
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
                Service.UIController.Update();
            }
        }, gatheringAddon->RootNode, NodePosition.AsLastChild);

        var maxGp = Service.ClientState.LocalPlayer?.MaxGp ?? 1500;
        Service.NativeController.AttachNode(sliderNode = new GpSliderNode((int)maxGp)
        {
            Position = new Vector2(320, 460),
            Size = new Vector2(200, 28),
            IsVisible = Service.Config.Display && Service.Config.DisplayGpSlider,
        }, gatheringAddon->RootNode, NodePosition.AsLastChild);
    }


    private unsafe void ClearUI(AtkUnitBase* addon)
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

        ClearRotations(addon);
    }
}
