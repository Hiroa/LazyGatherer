using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using Lumina.Excel.Sheets;

namespace LazyGatherer.Models;

public class ItemBase
{
    public required SeString Name { get; init; }
    public bool IsUnique { get; init; }
    public bool IsCollectable { get; init; }
    public byte FilterGroupId { get; init; }
    public uint ItemSearchCategoryId { get; init; }

    public static ItemBase FromLumina(Item item)
    {
        return new ItemBase
        {
            Name = item.Name.ToDalamudString(),
            IsUnique = item.IsUnique,
            IsCollectable = item.IsCollectable,
            FilterGroupId = item.FilterGroup,
            ItemSearchCategoryId = item.ItemSearchCategory.RowId,
        };
    }

    public static ItemBase FromLumina(EventItem item)
    {
        return new ItemBase
        {
            Name = item.Name.ToDalamudString(),
            IsUnique = false,
            IsCollectable = false,
            FilterGroupId = 12,       // 12 for log, 16 for ore
            ItemSearchCategoryId = 0, // Only used for Crystals(id 58)
        };
    }
}
