using System;

namespace LazyGatherer.Collectable.Conditions;

public class Condition
{
    public Operator Operator { get; set; } = Operator.Equal;
    public required Type Type { get; init; }
    public required object Value { get; set; }

    public bool IsSatisfied(Context ctx) => Type switch
    {
        Type.Attempt => CompareInt(ctx.Attempts, Convert.ToInt32(Value)),
        Type.CollectorStandard => Compare(ctx.HasCollectorStandard, Convert.ToBoolean(Value)),
        Type.Progression => CompareInt(ctx.Progression, Convert.ToInt32(Value)),
        _ => throw new ArgumentOutOfRangeException()
    };

    public bool Compare(bool value1, bool value2) => value1 == value2;

    public bool CompareInt(int value1, int value2) => Operator switch
    {
        Operator.LessThan => value1 < value2,
        Operator.LessThanOrEqual => value1 <= value2,
        Operator.Equal => value1 == value2,
        Operator.GreaterThan => value1 > value2,
        Operator.GreaterThanOrEqual => value1 >= value2,
        _ => throw new ArgumentOutOfRangeException()
    };
}
