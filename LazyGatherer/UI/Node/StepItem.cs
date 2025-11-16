using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace LazyGatherer.UI.Node;

public class StepItem : ButtonBase
{
    private NineGridNode backgroundNineGrid;
    private LabelTextNode itemNameLabel;
    private TextButtonNode editButton;

    public StepItem(int id)
    {
        SetInternalComponentType(ComponentType.Button);
        backgroundNineGrid = new SimpleNineGridNode
        {
            TexturePath = "ui/uld/BgParts_hr1.tex",
            TextureCoordinates = new Vector2(65, 65),
            TextureSize = new Vector2(20, 32),
            Offsets = new Vector4(10, 10, 9, 9),
        };
        itemNameLabel = new LabelTextNode
        {
            IsVisible = true,
            Position = new Vector2(5, 5),
            Size = new Vector2(200, 24),
            String = "Item Name " + id
        };
        editButton = new TextButtonNode
        {
            IsVisible = true,
            Position = new Vector2(220, 5),
            Size = new Vector2(50, 24),
            String = "Edit"
        };
        Service.NativeController.AttachNode(backgroundNineGrid, this);
        Service.NativeController.AttachNode(itemNameLabel, this);
        Service.NativeController.AttachNode(editButton, this);

        LoadTwoPartTimelines(this, backgroundNineGrid);
        InitializeComponentEvents();
    }

    public bool IsSelected
    {
        set => AddColor = value ? new Vector3(50, 25, 0) / 255 : new Vector3(0, 0, 0);
    }


    protected override void OnSizeChanged()
    {
        backgroundNineGrid.Size = Size;
        base.OnSizeChanged();
    }
}
