using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using LazyGatherer.Solver.Data;
using LazyGatherer.UI;

namespace LazyGatherer.Controller;

public class UIController(List<KeyValuePair<Rotation, GatheringOutcome>> outcomes) : IDisposable
{
    private readonly List<RotationNode> rotationNodes = [];

    public void Dispose()
    {
        ClearUi();
    }

    public unsafe void OnFrameworkUpdate()
    {
        if (outcomes.Count != rotationNodes.Count)
        {
            ClearUi();
            InitUi(outcomes);
        }

        var addonGathering = (AddonGathering*)Service.GameGui.GetAddonByName("Gathering");
        foreach (var rotationNode in rotationNodes)
        {
            if (rotationNode.IsVisible)
            {
                // Automatic gathering text node
                var autoGathering = addonGathering->UldManager.SearchNodeById(13);
                rotationNode.IsVisible =
                    !autoGathering->IsVisible(); //TODO  replace with the new CS AddonGathering->GatherStatus
            }
        }
    }

    public void Update()
    {
        rotationNodes.ForEach(r => r.Update());
    }

    private unsafe void InitUi(List<KeyValuePair<Rotation, GatheringOutcome>> gatheringOutcomes)
    {
        AtkUnitBase* gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering");
        foreach (var go in gatheringOutcomes)
        {
            try
            {
                var rotationNode = new RotationNode(go);
                rotationNodes.Add(rotationNode);
                Service.NativeController.AttachToAddon(rotationNode, gatheringAddon, gatheringAddon->RootNode,
                                                       NodePosition.AsLastChild);
            }
            catch (Exception e)
            {
                Service.Log.Error(e.ToString());
            }
        }
    }

    private unsafe void ClearUi()
    {
        try
        {
            AtkUnitBase* gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering");
            rotationNodes.ForEach(
                r => Service.NativeController.DetachFromAddon(r, gatheringAddon));
            rotationNodes.ForEach(r => r.Dispose());
            rotationNodes.Clear();
        }
        catch (Exception e)
        {
            Service.Log.Error(e.ToString());
        }
    }
}
