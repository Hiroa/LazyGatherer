using System.Collections.Generic;
using LazyGatherer.Solver.Collectable.Model.Actions;
using LazyGatherer.Solver.Collectable.Model.Conditions;

namespace LazyGatherer.Solver.Collectable.Model;

public class Turn
{
    public List<Condition> Conditions = [];
    public List<BaseAction> Actions = [];
}
