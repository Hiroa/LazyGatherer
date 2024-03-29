using System;
using System.Text;
using Lumina.Excel.GeneratedSheets2;

namespace LazyGatherer.Solver.Data;

public class GatheringContext
{
    // Row on the addon
    public required uint RowId { get; init; }

    public required Item Item { get; init; }
    
    // Available GP fro this gathering
    public required int AvailableGp { get; set; }

    // Base amount harvested by attempts (with node bonus)
    public required int BaseAmount { get; set; }
    
    // Chance to harvest item in percent
    public required double Chance { get; set; }
    
    // Attempts for this node
    public required int Attempts { get; set; }
    
    // Chance to get an extra item in percent
    public required double Boon { get; set; }
    public bool HasBoon { get; init; } = true;

    // Number of extra item retrieved with Bountiful
    public required int BountifulBonus { get; init; }
    
    // Number of extra item retrieved with Boon
    public int BoonBonus { get; set; } = 1;
    
    // Number of time bountiful has been used
    public int BountifulAttempts { get; set; }

    // Job level
    public required int CharacterLevel { get; init; }

    // Number of potential wise
    public int WiseAttempts { get; set; }
    
    public int GpRegenPerAttempt => CharacterLevel < 80 ? 5 : 6;
    public Job Job { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var property in this.GetType().GetProperties())
        {
            sb.Append(property.Name);
            sb.Append(": ");
            if (property.GetIndexParameters().Length > 0)
            {
                sb.Append("Indexed Property cannot be used");
            }
            else
            {
                sb.Append(property.GetValue(this, null));
            }

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }

    public GatheringContext Clone()
    {
        return (GatheringContext)this.MemberwiseClone();
    }
}
