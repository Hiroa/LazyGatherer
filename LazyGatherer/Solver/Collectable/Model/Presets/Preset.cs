using System.Collections.Generic;

namespace LazyGatherer.Solver.Collectable.Model.Presets;

public class Preset
{
    public required string Name { get; set; }
    public int MinLevel { get; set; } = 50;
    public int MinGp { get; set; }
    public List<Turn> Turns { get; } = [];
}
