using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;

namespace LazyGatherer.UI;

public abstract class CustomNode : SimpleComponentNode
{
    protected void AttachNode(NodeBase node)
    {
        Service.NativeController.AttachNode(node, this, NodePosition.AsLastChild);
    }
}
