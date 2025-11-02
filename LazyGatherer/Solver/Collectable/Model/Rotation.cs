using System.Collections.Generic;

namespace LazyGatherer.Solver.Collectable.Model;

public class Rotation
{
    public required Context Context { get; init; }
    public List<Turn> Turns { get; } = [];
}
