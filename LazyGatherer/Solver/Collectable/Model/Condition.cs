using System;

namespace LazyGatherer.Solver.Collectable.Model;

public class Condition
{
    public enum ConditionEnum
    {
        Attempt,
        CollectorStandard,
        Progression,
    }

    public enum ComparisonOperatorEnum
    {
        Lower,
        LowerOrEqual,
        Equal,
        Higher,
        HigherOrEqual
    }

    public ComparisonOperatorEnum ComparisonOperator { get; init; } = ComparisonOperatorEnum.Equal;
    public required ConditionEnum ConditionOn { get; init; }
    public required object Value { get; init; }

    public bool IsSatisfied(Context ctx)
    {
        Service.Log.Info("Checking condition on {0} with value {1} and operator {2}", ConditionOn, Value,
                         ComparisonOperator);
        return ConditionOn switch
        {
            ConditionEnum.Attempt => CompareInt(ctx.Attempts, Convert.ToInt32(Value)),
            ConditionEnum.CollectorStandard => Compare(ctx.HasCollectorStandard, Convert.ToBoolean(Value)),
            ConditionEnum.Progression => CompareInt(ctx.Progression, Convert.ToInt32(Value)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public bool Compare(bool value1, bool value2)
    {
        return value1 == value2;
    }

    public bool CompareInt(int value1, int value2)
    {
        return ComparisonOperator switch
        {
            ComparisonOperatorEnum.Lower => value1 < value2,
            ComparisonOperatorEnum.LowerOrEqual => value1 <= value2,
            ComparisonOperatorEnum.Equal => value1 == value2,
            ComparisonOperatorEnum.Higher => value1 > value2,
            ComparisonOperatorEnum.HigherOrEqual => value1 >= value2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
