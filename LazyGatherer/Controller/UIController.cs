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
    private ButtonNode? cog;

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

        var addonGathering = (AddonGathering*)Service.GameGui.GetAddonByName("Gathering").Address;
        foreach (var rotationNode in rotationNodes)
        {
            // Hide while quick gathering
            if (rotationNode.IsVisible && addonGathering->GatherStatus == 2)
            {
                rotationNode.IsVisible = false;
            }
        }
    }

    public void Update(bool clearNodes)
    {
        if (clearNodes)
        {
            this.ClearUi();
        }
        else
        {
            rotationNodes.ForEach(r => r.Update());
        }
    }

    private unsafe void InitUi(List<KeyValuePair<Rotation, GatheringOutcome>> gatheringOutcomes)
    {
        AtkUnitBase* gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering").Address;
        if (gatheringAddon == null || gatheringOutcomes.Count == 0)
        {
            return;
        }

        cog = new ButtonNode();
        Service.NativeController.AttachNode(cog, gatheringAddon->RootNode, NodePosition.AsLastChild);
        foreach (var go in gatheringOutcomes)
        {
            try
            {
                var rotationNode = new RotationNode(go);
                rotationNodes.Add(rotationNode);
                Service.NativeController.AttachNode(rotationNode, gatheringAddon->RootNode,
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
            AtkUnitBase* gatheringAddon = (AtkUnitBase*)Service.GameGui.GetAddonByName("Gathering").Address;
            if (gatheringAddon != null)
            {
                if (cog != null)
                {
                    Service.NativeController.DetachNode(cog);
                }

                rotationNodes.ForEach(r => Service.NativeController.DetachNode(r));
            }

            if (cog != null)
            {
                cog.Dispose();
                cog = null;
            }

            rotationNodes.ForEach(r => r.Dispose());
            rotationNodes.Clear();
        }
        catch (Exception e)
        {
            Service.Log.Error(e.ToString());
        }
    }
}
