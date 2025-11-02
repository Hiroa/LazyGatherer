using System.Collections.Generic;
using LazyGatherer.Solver.Collectable.Model.Actions;

namespace LazyGatherer.Solver.Collectable.Model;

public class Turn
{
    public List<Condition> Conditions { get; } = [];
    public List<BaseAction> Actions { get; } = [];
}
