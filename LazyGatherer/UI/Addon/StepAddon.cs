using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Nodes;

namespace LazyGatherer.UI.Addon;

public class StepAddon : NativeAddon
{
    private HorizontalListNode? actionsListNode;
    private HorizontalListNode? bonusListNode;
    private int selectedAction;

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        SetWindowSize(new Vector2(300.0f, 150.0f));
        // Add Action Nodes

        actionsListNode = new HorizontalListNode
        {
            IsVisible = true,
            Height = 48,
        };
        actionsListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(48, 48),
            ActionId = 22182, // Scour
            OnClick = () => { ToggleActionsNode(0); }
        });
        actionsListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(48, 48),
            ActionId = 22183u, // Brazen
            OnClick = () => ToggleActionsNode(1),
            Enabled = false
        });
        actionsListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(48, 48),
            ActionId = 22184u, // Meticulous
            OnClick = () => ToggleActionsNode(2),
            Enabled = false
        });
        actionsListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(48, 48),
            ActionId = 240u, // Collect
            OnClick = () => ToggleActionsNode(3),
            Enabled = false
        });

        AttachNode(actionsListNode);
        AttachNode(new HorizontalLineNode()
        {
            IsVisible = true,
        });

        // Add Bonus Action Nodes
        bonusListNode = new HorizontalListNode
        {
            IsVisible = true,
            Height = 48,
        };
        bonusListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(48, 48),
            ActionId = 22185u, // Scrutiny
            CanBeToggle = true,
            Enabled = false
        });
        bonusListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(48, 48),
            ActionId = 21205u, // Focus
            CanBeToggle = true,
            Enabled = false
        });
        bonusListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(48, 48),
            ActionId = 34871u, // Priming
            CanBeToggle = true,
            Enabled = false
        });
        bonusListNode.AddNode(new ActionNode
        {
            IsVisible = true,
            Size = new Vector2(24, 48),
            ActionId = 232, // Attempt
            CanBeToggle = true,
            Enabled = false
        });
        AttachNode(bonusListNode);
    }


    /**
     * Disable all action nodes except the one clicked
     */
    private void ToggleActionsNode(int i)
    {
        foreach (var actionNode in (actionsListNode?.Nodes as List<ActionNode>)!)
        {
            actionNode.Enabled = false;
        }

        selectedAction = i;
        (actionsListNode.Nodes[i] as ActionNode)!.Enabled = true;
    }
}
