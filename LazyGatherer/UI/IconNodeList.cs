using KamiToolKit.Nodes;

namespace LazyGatherer.UI;

public class IconNodeList : ListNode<IconNode>
{
    protected override string GetLabelForOption(IconNode option)
    {
        return "GetLabelForOption";
    }
}
