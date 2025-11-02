using System;

namespace LazyGatherer.Solver.Collectable.Model.Conditions;

public class ProgressionCondition : Condition
{
    public int RequiredProgression { get; init; }
    public Equality Equality { get; init; }

    public override bool IsSatisfied(Context ctx)
    {
        return Equality switch
        {
            Equality.Lower => ctx.Progression < RequiredProgression,
            Equality.LowerOrEqual => ctx.Progression <= RequiredProgression,
            Equality.Equal => ctx.Progression == RequiredProgression,
            Equality.Higher => ctx.Progression > RequiredProgression,
            Equality.HigherOrEqual => ctx.Progression >= RequiredProgression,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
