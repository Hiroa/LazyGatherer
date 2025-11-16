using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace LazyGatherer.UI.Node;

public class StepList : SimpleComponentNode
{
    private NineGridNode backgroundNineGrid;
    private List<StepItem> renderers = [];
    private TextureButtonNode plusButton;
    private TextureButtonNode minusButton;
    private TextureButtonNode upButton;
    private TextureButtonNode downButton;
    private int selectedStep = -1;
    private const float StepItemHeight = 34.0f;

    public StepList()
    {
        SetInternalComponentType(ComponentType.Base);
        backgroundNineGrid = new SimpleNineGridNode
        {
            TexturePath = "ui/uld/BgParts_hr1.tex",
            TextureCoordinates = new Vector2(61, 37),
            TextureSize = new Vector2(16, 16),
            Offsets = new Vector4(7),
        };
        Service.NativeController.AttachNode(backgroundNineGrid, this);
        for (var i = 0; i < 5; i++)
        {
            AddItem(i);
        }

        plusButton = new TextureButtonNode
        {
            IsVisible = true,
            Position = new Vector2(5, 5),
            Size = new Vector2(28, 28),
            TexturePath = "ui/uld/NumericStepperB_hr1.tex",
            TextureCoordinates = new Vector2(28, 0),
            TextureSize = new Vector2(28, 28),
            OnClick = () =>
            {
                AddItem(renderers.Count);
                UpdateList();
            },
        };
        Service.NativeController.AttachNode(plusButton, this);

        minusButton = new TextureButtonNode
        {
            IsVisible = true,
            Position = new Vector2(5, 5),
            Size = new Vector2(28, 28),
            TexturePath = "ui/uld/NumericStepperB_hr1.tex",
            TextureCoordinates = new Vector2(0, 0),
            TextureSize = new Vector2(28, 28),
            OnClick = () =>
            {
                if (selectedStep < 0 || selectedStep >= renderers.Count)
                    return;

                var toRemove = renderers[selectedStep];
                Service.NativeController.DetachNode(toRemove);
                renderers.Remove(toRemove);
                selectedStep = -1;
                UpdateList();
            },
        };
        Service.NativeController.AttachNode(minusButton, this);

        upButton = new TextureButtonNode
        {
            IsVisible = true,
            Position = new Vector2(5, 5),
            Size = new Vector2(50, 24),
            TexturePath = "ui/uld/SubCommandSetting_hr1.tex",
            TextureCoordinates = new Vector2(0, 24),
            TextureSize = new Vector2(50, 24),
            OnClick = () =>
            {
                if (selectedStep <= 0 || selectedStep >= renderers.Count)
                    return;
                var stepItemToMoveUp = renderers[selectedStep];
                var stepItemAbove = renderers[selectedStep - 1];
                renderers[selectedStep - 1] = stepItemToMoveUp;
                renderers[selectedStep] = stepItemAbove;
                selectedStep--;
                UpdateList();
            },
        };
        Service.NativeController.AttachNode(upButton, this);

        downButton = new TextureButtonNode
        {
            IsVisible = true,
            Position = new Vector2(5, 5),
            Size = new Vector2(50, 24),
            TexturePath = "ui/uld/SubCommandSetting_hr1.tex",
            TextureCoordinates = new Vector2(0, 48),
            TextureSize = new Vector2(50, 24),
            OnClick = () =>
            {
                if (selectedStep < 0 || selectedStep >= renderers.Count - 1)
                    return;
                var stepItemToMoveDown = renderers[selectedStep];
                var stepItemBelow = renderers[selectedStep + 1];
                renderers[selectedStep + 1] = stepItemToMoveDown;
                renderers[selectedStep] = stepItemBelow;
                selectedStep++;
                UpdateList();
            },
        };
        Service.NativeController.AttachNode(downButton, this);
    }

    private void AddItem(int i)
    {
        var id = i; // To capture the correct index
        var renderer = new StepItem(id)
        {
            IsVisible = true,
            Position = new Vector2(5, (i * StepItemHeight) + 5),
            OnClick = () => OnClick(id),
            Size = Size
        };
        renderers.Add(renderer);
        Service.NativeController.AttachNode(renderer, this);
    }

    private void UpdateList()
    {
        for (var i = 0; i < renderers.Count; i++)
        {
            var stepItem = renderers[i];
            stepItem.Size = Size with { Y = StepItemHeight } - new Vector2(60, 0);
            stepItem.Position = new Vector2(5, (i * StepItemHeight) + 5);
            var id = i;                           // To capture the correct index
            stepItem.OnClick = () => OnClick(id); // Update OnClick to use the new index
        }
    }

    protected override void OnSizeChanged()
    {
        backgroundNineGrid.Size = Size - new Vector2(50, 0);
        UpdateList();

        plusButton.Position = new Vector2(Width - 50, 0);
        minusButton.Position = new Vector2(Width - 28, 0);
        upButton.Position = new Vector2(Width - 50, 28);
        downButton.Position = new Vector2(Width - 50, 52);
        base.OnSizeChanged();
    }

    private void OnClick(int index)
    {
        selectedStep = index;
        for (var i = 0; i < renderers.Count; i++)
        {
            renderers[i].IsSelected = i == selectedStep;
        }
    }
}
