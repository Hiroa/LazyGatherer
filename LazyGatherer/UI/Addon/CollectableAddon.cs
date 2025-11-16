using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Nodes;
using LazyGatherer.UI.Node;

namespace LazyGatherer.UI.Addon;

public class CollectableAddon : NativeAddon
{
    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        SetWindowSize(new Vector2(400.0f, 400.0f));
        // Text input for the name
        AttachNode(new TextInputNode
        {
            IsVisible = true,
            Position = ContentStartPosition,
            Size = new Vector2(152.0f, 28.0f),
        });

        AttachNode(new NumericInputNode
        {
            IsVisible = true,
            Position = ContentStartPosition + new Vector2(0, 24),
            Size = new Vector2(200.0f, 24.0f),
        });

        AttachNode(new NumericInputNode
        {
            IsVisible = true,
            Position = ContentStartPosition + new Vector2(0, 48),
            Size = new Vector2(200.0f, 24.0f),
        });

        AttachNode(new StepList
        {
            IsVisible = true,
            Position = ContentStartPosition + new Vector2(0, 72),
            Size = new Vector2(380.0f, 270.0f),
        });
    }
}
