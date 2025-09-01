using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace LazyGatherer.Components;

public unsafe class GatheringItemComponent(AddonGathering* addon, int rowId)
{
    public AtkComponentCheckBox* Component { get; set; } = addon->GatheredItemComponentCheckbox[rowId];

    public bool IsRare => Component->UldManager.SearchNodeById(7)->IsVisible();
    public int GatheringChance => Component->UldManager.SearchNodeById(10)->GetAsAtkTextNode()->NodeText.ToInteger();
    private Utf8String BoonText => Component->UldManager.SearchNodeById(16)->GetAsAtkTextNode()->NodeText;
    public bool HasBoon => !BoonText.EqualToString("-");
    public int BoonChance => HasBoon ? BoonText.ToInteger() : 0;
    private AtkComponentIcon* GetIconNode() => Component->UldManager.SearchNodeById(31)->GetAsAtkComponentIcon();
    private Utf8String BaseAmountText => GetIconNode()->UldManager.SearchNodeById(7)->GetAsAtkTextNode()->NodeText;
    public int BaseAmount => BaseAmountText.EqualToString("") ? 1 : BaseAmountText.ToInteger();
}
