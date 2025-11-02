using System;
using System.Text;
using LazyGatherer.Models;

namespace LazyGatherer.Solver.Collectable.Model;

public class Context
{
    // Player info
    // Job level
    public required int CharacterLevel { get; init; }
    public Job Job { get; init; }

    // Available GP for this gathering
    public required int AvailableGp { get; set; }

    // Gathering info
    // Item to gather
    public required string ItemName { get; init; }

    // Gathering progression on 1000
    public int Progression { get; set; }

    // Chance to harvest item in percent
    public required double Chance { get; set; }

    // Attempts for this node
    public required int Attempts { get; set; }
    public required int MaxAttempts { get; set; }

    // Gp regenerated per attempt
    public int GpRegenPerAttempt => CharacterLevel < 80 ? 5 : 6;

    // Gathering buff
    public required bool HasCollectorStandard { get; init; }
    public required bool HasScrutiny { get; init; }
    public required bool HasEureka { get; init; }

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

    public Context Clone()
    {
        return (Context)this.MemberwiseClone();
    }
}
