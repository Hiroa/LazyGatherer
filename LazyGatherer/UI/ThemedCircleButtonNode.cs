using KamiToolKit.Nodes;

namespace LazyGatherer.UI;

public class ThemedCircleButtonNode : CircleButtonNode
{
    public void LoadTheme(uint colorTheme)
    {
        var theme = colorTheme switch
        {
            0 => "",
            1 => "img01/",
            2 => "img02/",
            3 => "img03/",
            _ => "",
        };
        ImageNode.TexturePath = $"ui/uld/{theme}CircleButtons.tex";
    }
}
