using System.Collections.Generic;
using LazyGatherer.Collectable.Actions;
using LazyGatherer.Collectable.Conditions;

namespace LazyGatherer.Collectable;

public class Step
{
    public List<CollectableAction.Names> Actions { get; } = [];
    public List<Condition> Conditions { get; } = [];
}
