using System.Collections.Generic;
using LazyGatherer.Solver.Collectable.Model.Presets;

namespace LazyGatherer.Solver.Collectable.Model;

public class Rotation
{
    public required Context Context { get; init; }
    public List<Step> Turns { get; init; } = [];

    public static Rotation FromPreset(Preset preset, Context ctx)
    {
        return new Rotation
        {
            Context = ctx,
            Turns = preset.Turns
        };
    }
}
