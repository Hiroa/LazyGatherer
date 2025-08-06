using KamiToolKit.Nodes;
using KamiToolKit.System;

namespace LazyGatherer.UI;

public class IconNodeList : ListNode<NodeBase>
{
    protected override string GetLabelForOption(NodeBase option)
    {
        return "GetLabelForOption";
    }
}
