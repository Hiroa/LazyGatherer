using System.Drawing;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Config;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes;

namespace LazyGatherer.UI;

public sealed class ButtonNode : ComponentNode<AtkComponentButton, AtkUldComponentDataButton>
{
    private const uint ButtonNodeId = 7_000;
    private readonly ImageNode imageNode;

    public ButtonNode()
    {
        SetInternalComponentType(ComponentType.Button);
        var nodeId = ButtonNodeId;
        IsVisible = true;
        Position = new Vector2(443.0f, 6.0f);
        Size = new Vector2(28, 28);
        Tooltip = "LazyGatherer Configuration";

        imageNode = new ImageNode()
        {
            Color = KnownColor.White.Vector(),
            Width = 28,
            Height = 28,
            ImageNodeFlags = 0,
            IsVisible = true,
        };
        var imagePart = new Part()
        {
            Id = 5000,
            Size = new Vector2(28, 28),
            TextureCoordinates = new Vector2(0, 0)
        };

        Service.GameConfig.TryGet(SystemConfigOption.ColorThemeType, out uint colorTheme);
        switch (colorTheme)
        {
            case 0:
                imagePart.LoadTexture("ui/uld/CircleButtons.tex");
                break;
            // case 1:
            //     imagePart.LoadTexture("ui/uld/light/CircleButtons.tex");
            //     break;
            // case 2:
            // imagePart.LoadTexture("ui/uld/img03/CircleButtons.tex");
            // break;
            // case 3:
            // imagePart.LoadTexture("ui/uld/fourth/CircleButtons.tex");
            // break;
            default:
                imagePart.LoadTexture("ui/uld/CircleButtons.tex");
                break;
        }

        imageNode.AddPart(imagePart);


        Service.NativeController.AttachNode(imageNode, this, NodePosition.AsFirstChild);


        AddEvent(AddonEventType.MouseOver, _ => imageNode.AddColor = new Vector3(0.1f, 0.1f, 0.1f));
        AddEvent(AddonEventType.MouseOut, _ => imageNode.AddColor = Vector3.Zero);
        AddEvent(AddonEventType.ButtonClick, _ => Service.ConfigController.ToggleConfigWindow());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            imageNode.Dispose();
            base.Dispose(disposing);
        }
    }
}
