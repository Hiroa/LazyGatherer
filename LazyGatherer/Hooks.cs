using System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace LazyGatherer;

public unsafe class Hooks : IDisposable
{
    public delegate byte IsActionHighlightedDelegate(ActionManager* manager, ActionType actionType, uint actionId);

    public readonly Hook<IsActionHighlightedDelegate> IsActionHighlightedHook = null!;

    public Hooks()
    {
        IsActionHighlightedHook =
            Service.GameInteropProvider.HookFromAddress<IsActionHighlightedDelegate>(
                (nint)ActionManager.MemberFunctionPointers.IsActionHighlighted, IsActionHighlightedDetour);
        IsActionHighlightedHook.Enable();
    }

    private byte IsActionHighlightedDetour(ActionManager* manager, ActionType actionType, uint actionId)
    {
        var result = IsActionHighlightedHook.Original(manager, actionType, actionId);
        return Service.MasterpieceController.CurrActionId == actionId ? (byte)1 : result;
    }


    public void Dispose()
    {
        IsActionHighlightedHook.Dispose();
    }
}
