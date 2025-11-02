using System.Collections.Generic;
using LazyGatherer.Models;
using LazyGatherer.Solver.Models;

namespace LazyGatherer.Solver.Comparator;

public abstract class RotationComparer(ComparerEnum name) : IComparer<GatheringOutcome>
{
    public ComparerEnum Name { get; init; } = name;
    public abstract int Compare(GatheringOutcome? x, GatheringOutcome? y);
}
