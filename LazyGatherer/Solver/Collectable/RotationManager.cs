using System.Linq;
using LazyGatherer.Solver.Collectable.Model;
using LazyGatherer.Solver.Collectable.Model.Actions;
using LazyGatherer.Solver.Collectable.Model.Conditions;

namespace LazyGatherer.Solver.Collectable;

public class RotationManager
{
    public static BaseAction? GetNextAction(Rotation rotation)
    {
        foreach (var turn in rotation.Turns)
        {
            if (turn.Conditions.All(c => c.IsSatisfied(rotation.Context)))
            {
                var action = turn.Actions.FirstOrDefault(a => a.CanExecute(rotation));
                if (action != null)
                {
                    return action;
                }
            }
        }

        return null;
    }

    public static bool IsValidRotation(Rotation rotation)
    {
        foreach (var turn in rotation.Turns)
        {
            var count = turn.Actions.Select(a => a.IsEndingTurn).Count();
            if (count > 1)
            {
                Service.Log.Info("Invalid rotation: More than one ending turn action in a single turn.");
                return false;
            }
        }

        return true;
    }

    public static Rotation BasicRotation(Context ctx)
    {
        var r = new Rotation
        {
            Context = ctx,
            Turns =
            {
                new Turn // Reach 1000 progression
                {
                    Actions =
                    {
                        new Attempt(),
                        new Collect()
                    },
                    Conditions =
                    {
                        new ProgressionCondition
                        {
                            RequiredProgression = 1000,
                            Equality = Equality.HigherOrEqual
                        }
                    }
                },
                new Turn // Last attempt collect
                {
                    Actions =
                    {
                        new Collect()
                    },
                    Conditions =
                    {
                        new AttemptsConditions
                        {
                            AttemptsAmount = 1,
                            Equality = Equality.LowerOrEqual
                        }
                    }
                },
                new Turn // Meticulous until 800 progression
                {
                    Actions =
                    {
                        new Scrutiny(),
                        new Meticulous()
                    },
                    Conditions =
                    {
                        new ProgressionCondition
                        {
                            RequiredProgression = 800,
                            Equality = Equality.Lower
                        },
                        new CollectorStandardCondition
                        {
                            ShouldHaveCollectorStandard = false
                        }
                    }
                },
                new Turn // Brazen + Scrutiny until 800 progression if have Collector's Standard
                {
                    Actions =
                    {
                        new Scrutiny(),
                        new Brazen()
                    },
                    Conditions =
                    {
                        new ProgressionCondition
                        {
                            RequiredProgression = 800,
                            Equality = Equality.Lower
                        },
                        new CollectorStandardCondition
                        {
                            ShouldHaveCollectorStandard = true
                        }
                    }
                },
                new Turn() // Meticulous if progression >= 800 and Collector's Standard
                {
                    Actions =
                    {
                        new Meticulous()
                    },
                    Conditions =
                    {
                        new ProgressionCondition
                        {
                            RequiredProgression = 800,
                            Equality = Equality.HigherOrEqual
                        },
                        new CollectorStandardCondition
                        {
                            ShouldHaveCollectorStandard = true
                        }
                    }
                },
                new Turn() // Meticulous if progression >= 850
                {
                    Actions =
                    {
                        new Meticulous()
                    },
                    Conditions =
                    {
                        new ProgressionCondition
                        {
                            RequiredProgression = 850,
                            Equality = Equality.HigherOrEqual
                        }
                    }
                },
                new Turn() // Scour if progression > 800 and no Collector's Standard
                {
                    Actions =
                    {
                        new Scour()
                    },
                    Conditions =
                    {
                        new ProgressionCondition
                        {
                            RequiredProgression = 800,
                            Equality = Equality.Higher
                        },
                        new CollectorStandardCondition
                        {
                            ShouldHaveCollectorStandard = false
                        }
                    }
                }
            }
        };
        return r;
    }
}
