namespace LazyGatherer.Solver.Collectable.Model.Conditions;

public abstract class Condition
{
    public abstract bool IsSatisfied(Context ctx);
}
