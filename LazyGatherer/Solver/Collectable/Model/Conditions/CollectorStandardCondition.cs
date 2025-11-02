namespace LazyGatherer.Solver.Collectable.Model.Conditions;

public class CollectorStandardCondition : Condition
{
    public bool ShouldHaveCollectorStandard { get; init; }

    public override bool IsSatisfied(Context ctx)
    {
        return ctx.HasCollectorStandard == ShouldHaveCollectorStandard;
    }
}
