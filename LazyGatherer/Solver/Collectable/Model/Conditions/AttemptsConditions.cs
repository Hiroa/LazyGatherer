using System;

namespace LazyGatherer.Solver.Collectable.Model.Conditions;

public class AttemptsConditions : Condition
{
    public int AttemptsAmount { get; init; }
    public Equality Equality { get; init; }

    public override bool IsSatisfied(Context ctx)
    {
        return Equality switch
        {
            Equality.Lower => ctx.Attempts < AttemptsAmount,
            Equality.LowerOrEqual => ctx.Attempts <= AttemptsAmount,
            Equality.Equal => ctx.Attempts == AttemptsAmount,
            Equality.Higher => ctx.Attempts > AttemptsAmount,
            Equality.HigherOrEqual => ctx.Attempts >= AttemptsAmount,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
