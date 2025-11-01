using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using LazyGatherer.Models;
using LazyGatherer.Solver.Gathering.Actions;
using LazyGatherer.Solver.Gathering.Models;

namespace LazyGatherer.UI;

public sealed class ActionNode : CustomNode
{
    private readonly IconImageNode iconNode;
    private readonly SimpleImageNode backgroundNode;
    private readonly TextNode countNode;

    public ActionNode(GatheringContext context, BaseAction action, int count)
    {
        // Icon node for action
        AttachNode(iconNode = new IconImageNode()
        {
            Position = new Vector2(4f, 4f),
            Size = new Vector2(40, 40),
            TextureSize = new Vector2(40, 40),
            IconId = GetGathererIconId(action, context),
            IsVisible = true
        });

        // Background node for icon
        AttachNode(backgroundNode = new SimpleImageNode()
        {
            Position = new Vector2(0, 0),
            Size = new Vector2(48, 48),
            TextureCoordinates = new Vector2(0, 0),
            TextureSize = new Vector2(48, 48),
            IsVisible = true,
            ImageNodeFlags = 0
        });
        backgroundNode.LoadTexture("ui/uld/IconA_Frame.tex");

        // Text node for count
        AttachNode(countNode = new TextNode
        {
            AlignmentType = AlignmentType.Right,
            Position = new Vector2(4f, 4f),
            Size = new Vector2(40, 14),
            SeString = $"x{count}",
            TextFlags = TextFlags.Edge,
            FontSize = 14,
            IsVisible = count > 1
        });
    }

    private static uint GetGathererIconId(BaseAction action, GatheringContext context) => context.Job switch
    {
        Job.Min => action.MinerAction.Icon,
        Job.Bot => action.BotanistAction.Icon,
        _ => 0u
    };
}
