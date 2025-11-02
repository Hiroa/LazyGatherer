using System.Collections.Generic;

namespace LazyGatherer.Solver.Collectable.Model;

public class Rotation
{
    public Context Context { get; init; }
    public List<Turn> Turns { get; } = [];
}
