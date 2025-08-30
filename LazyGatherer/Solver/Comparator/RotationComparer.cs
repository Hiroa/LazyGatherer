using System.Collections.Generic;
using LazyGatherer.Solver.Data;

namespace LazyGatherer.Solver.Comparator;

public abstract class RotationComparer(string name) : IComparer<GatheringOutcome>
{
    public string Name { get; init; } = name;
    public abstract int Compare(GatheringOutcome? x, GatheringOutcome? y);
}
