using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Controllers;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using LazyGatherer.Gathering.Models;
using LazyGatherer.UI.Node;

namespace LazyGatherer.Controller;

public class UIController : IDisposable
{
    private readonly AddonController addonController;
    private readonly List<RotationNode> rotationNodes = [];
    private CircleButtonNode? configButtonNode;
    private CircleButtonNode? displayButtonNode;
    private CircleButtonNode? gpAlertButtonNode;
    private GpSliderNode? sliderNode;

    public unsafe UIController()
    {
        addonController = new AddonController
        {
            AddonName = "Gathering",
            OnSetup = OnGatheringPostSetup,
            OnFinalize = OnGatheringPreFinalize,
            OnUpdate = OnGatheringPostUpdate
        };
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
            if (gpAlertButtonNode is { IsVisible: true }) gpAlertButtonNode.IsVisible = false;
            if (sliderNode is { IsVisible: true }) sliderNode.IsVisible = false;
            rotationNodes.ForEach(r => r.IsVisible = false);
        }
    }

    public void Update()
    {
        sliderNode?.IsVisible = Service.Config.Display && Service.Config.DisplayGpSlider;

        rotationNodes.ForEach(r => r.Update());
        // Uniform sizing
        var biggestNodeSize = rotationNodes.MaxBy(r => r.Size.X)?.Size ?? Vector2.Zero;
        rotationNodes.ForEach(r => r.Size = biggestNodeSize);
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

        var biggestNodeSize = Vector2.Zero;
        foreach (var go in gatheringOutcomes)
        {
            var rotationNode = new RotationNode(go);
            rotationNodes.Add(rotationNode);
            rotationNode.AttachNode(gatheringAddon->RootNode);
            // Determine biggest node for uniform sizing
            if (rotationNode.Size.X > biggestNodeSize.X)
                biggestNodeSize = rotationNode.Size;
        }

        // Update all nodes to the biggest size
        rotationNodes.ForEach(r => r.Size = biggestNodeSize);
    }

    private unsafe void ClearRotations(AtkUnitBase* gatheringAddon)
    {
        if (gatheringAddon != null)
        {
            rotationNodes.ForEach(r => r.DetachNode());
        }

        rotationNodes.ForEach(r => r.Dispose());
        rotationNodes.Clear();
    }

    private unsafe void InitUI(AtkUnitBase* gatheringAddon)
    {
        if (gatheringAddon == null)
            return;

        configButtonNode = new CircleButtonNode
        {
            Position = new Vector2(450.0f, 8.0f),
            Size = new Vector2(24f, 24f),
            Icon = CircleButtonIcon.GearCog,
            TextTooltip = "[LazyGatherer] Configuration",
            IsVisible = true,
            OnClick = () => Service.ConfigAddon.Toggle(),
        };
        configButtonNode.AttachNode(gatheringAddon->RootNode);


        displayButtonNode = new CircleButtonNode
        {
            Position = new Vector2(428.0f, 8.0f),
            Size = new Vector2(24f, 24f),
            Icon = CircleButtonIcon.Eye,
            TextTooltip = "[LazyGatherer] Toggle display",
            IsVisible = true,
            OnClick = () =>
            {
                Service.Config.Display = !Service.Config.Display;
                Service.Interface.SavePluginConfig(Service.Config);
                Service.UIController.Update();
            }
        };
        displayButtonNode.AttachNode(gatheringAddon->RootNode);

        gpAlertButtonNode = new CircleButtonNode
        {
            Position = new Vector2(406.0f, 8.0f),
            Size = new Vector2(24f, 24f),
            Icon = CircleButtonIcon.Volume,
            TextTooltip = "[LazyGatherer] GP alert config",
            IsVisible = true,
            OnClick = () => { Service.GpAlertAddon.Toggle(); }
        };
        gpAlertButtonNode.AttachNode(gatheringAddon->RootNode);

        var maxGp = Service.ObjectTable.LocalPlayer?.MaxGp ?? 1500;
        sliderNode = new GpSliderNode((int)maxGp)
        {
            Position = new Vector2(320, 460),
            Size = new Vector2(200, 28),
            IsVisible = Service.Config.Display && Service.Config.DisplayGpSlider,
        };
        sliderNode.AttachNode(gatheringAddon->RootNode);
    }


    private unsafe void ClearUI(AtkUnitBase* addon)
    {
        if (configButtonNode != null)
        {
            configButtonNode.DetachNode();
            configButtonNode.Dispose();
            configButtonNode = null;
        }

        if (displayButtonNode != null)
        {
            displayButtonNode.DetachNode();
            displayButtonNode.Dispose();
            displayButtonNode = null;
        }

        if (gpAlertButtonNode != null)
        {
            gpAlertButtonNode.DetachNode();
            gpAlertButtonNode.Dispose();
            gpAlertButtonNode = null;
        }

        if (sliderNode != null)
        {
            sliderNode.DetachNode();
            sliderNode.Dispose();
            sliderNode = null;
        }

        ClearRotations(addon);
    }
}
