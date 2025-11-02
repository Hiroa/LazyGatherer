namespace LazyGatherer.Solver.Collectable.Model.Conditions;

public class ScrutinyCondition : Condition
{
    public bool ShouldHaveScrutiny { get; init; }

    public override bool IsSatisfied(Context ctx)
    {
        return ctx.HasScrutiny == ShouldHaveScrutiny;
    }
}
