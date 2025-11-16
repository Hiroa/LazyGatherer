using System.Collections.Generic;
using LazyGatherer.Solver.Collectable.Model.Actions;

namespace LazyGatherer.Solver.Collectable.Model;

public class Step
{
    public string Description { get; set; } = "";
    public List<Condition> Conditions { get; } = [];
    public List<BaseAction> Actions { get; } = [];
}
