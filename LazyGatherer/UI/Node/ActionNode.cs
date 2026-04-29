using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using Lumina.Excel.Sheets;
using Vector3 = System.Numerics.Vector3;

namespace LazyGatherer.UI.Node;

public sealed class ActionNode : ButtonBase
{
    private readonly IconImageNode iconNode;
    private readonly SimpleImageNode backgroundNode;
    private readonly TextNode countNode;

    public ActionNode()
    {
        // Icon node for action
        iconNode = new IconImageNode()
        {
            Position = new Vector2(4f, 4f),
            Size = new Vector2(40, 40),
            TextureSize = new Vector2(40, 40),
            IsVisible = true
        };
        iconNode.AttachNode(this);

        // Background node for icon
        backgroundNode = new SimpleImageNode()
        {
            Position = new Vector2(0, 0),
            Size = new Vector2(48, 48),
            TextureCoordinates = new Vector2(0, 0),
            TextureSize = new Vector2(48, 48),
            IsVisible = true,
            ImageNodeFlags = 0
        };
        backgroundNode.LoadTexture("ui/uld/IconA_Frame.tex");
        backgroundNode.AttachNode(this);

        // Text node for count
        countNode = new TextNode
        {
            AlignmentType = AlignmentType.Right,
            Position = new Vector2(4f, 4f),
            Size = new Vector2(40, 14),
            TextFlags = TextFlags.Edge,
            FontSize = 14,
        };
        countNode.AttachNode(this);

        InitializeComponentEvents();
    }

    public int Count
    {
        set
        {
            countNode.String = $"x{value}";
            countNode.IsVisible = value > 1;
        }
    }

    public bool Enabled
    {
        get => iconNode.MultiplyColor.X >= 1f;
        set => iconNode.MultiplyColor = value ? new Vector3(1f) : new Vector3(0.5f);
    }

    public uint ActionId
    {
        set
        {
            var action = Service.DataManager.Excel.GetSheet<Action>()[value];
            iconNode.LoadIcon(action.Icon);
            TextTooltip = action.Name.ToDalamudString().TextValue;
        }
    }

    public bool CanBeToggle
    {
        set { OnClick = value ? () => Enabled = !Enabled : null; }
    }
}
