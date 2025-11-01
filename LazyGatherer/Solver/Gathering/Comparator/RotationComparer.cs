using System.Collections.Generic;
using LazyGatherer.Solver.Gathering.Models;

namespace LazyGatherer.Solver.Gathering.Comparator;

public abstract class RotationComparer(ComparerEnum name) : IComparer<GatheringOutcome>
{
    public ComparerEnum Name { get; init; } = name;
    public abstract int Compare(GatheringOutcome? x, GatheringOutcome? y);
}
