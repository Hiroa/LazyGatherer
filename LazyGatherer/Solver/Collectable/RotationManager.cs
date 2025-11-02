using System;
using System.Linq;
using Dalamud.Utility;
using LazyGatherer.Solver.Collectable.Model;
using LazyGatherer.Solver.Collectable.Model.Actions;
using LazyGatherer.Solver.Collectable.Model.Presets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Turn = LazyGatherer.Solver.Collectable.Model.Turn;

namespace LazyGatherer.Solver.Collectable;

public static class RotationManager
{
    private static readonly Wise Wise = new();

    public static BaseAction? GetNextAction(Rotation rotation)
    {
        Service.Log.Info("Determining next action...\n" + rotation.Context);
        // Prioritize wise if not max attempts - no matter the rotation
        if (rotation.Context.Attempts < rotation.Context.MaxAttempts
            && rotation.Context.HasEureka)
        {
            return Wise;
        }

        var count = 0;
        foreach (var turn in rotation.Turns)
        {
            Service.Log.Info("Checking turn " + count++);
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
            var actions = turn.Actions.Where(a => a.IsEndingTurn).ToList();
            if (actions.Count > 1)
            {
                Service.Log.Info("Invalid rotation: More than one ending turn action in a single turn.");
                Service.Log.Info("Turn actions: " + string.Join(", ", actions));
                return false;
            }
        }

        return true;
    }

    public static Rotation Import(string str, Context ctx)
    {
        try
        {
            var importStr = Util.DecompressString(Convert.FromBase64String(str));
            var preset = JsonConvert.DeserializeObject<Preset>(importStr, JsonSettings);
            if (preset == null)
            {
                throw new Exception("Deserialized preset is null");
            }

            // Service.Log.Info("Imported successfully");
            return Rotation.FromPreset(preset, ctx);
        }
        catch (Exception ex)
        {
            Service.Log.Error($"Error importing rotation!\n{ex}");
            return BasicRotation(ctx);
        }
    }

    public static Rotation BasicRotation(Context ctx)
    {
        var template = new Preset
        {
            Name = "1000 Gp rotation from TC",
            MinGp = 1000,
            MinLevel = 50,
            Turns =
            {
                new Turn // Reach 1000 progression
                {
                    Actions =
                    {
                        BaseAction.Actions[BaseAction.CollectableAction.Attempt],
                        BaseAction.Actions[BaseAction.CollectableAction.Collect],
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.Progression,
                            Value = 1000,
                            ComparisonOperator = Condition.ComparisonOperatorEnum.HigherOrEqual
                        }
                    }
                },
                new Turn // Last attempt collect
                {
                    Actions =
                    {
                        BaseAction.Actions[BaseAction.CollectableAction.Collect],
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.Attempt,
                            Value = 1,
                            ComparisonOperator = Condition.ComparisonOperatorEnum.LowerOrEqual
                        }
                    }
                },
                new Turn // Meticulous until 800 progression
                {
                    Actions =
                    {
                        BaseAction.Actions[BaseAction.CollectableAction.Scrutiny],
                        BaseAction.Actions[BaseAction.CollectableAction.Meticulous],
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.Progression,
                            Value = 800,
                            ComparisonOperator = Condition.ComparisonOperatorEnum.Lower
                        },
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.CollectorStandard,
                            Value = false
                        }
                    }
                },
                new Turn // Brazen + Scrutiny until 800 progression if have Collector's Standard
                {
                    Actions =
                    {
                        BaseAction.Actions[BaseAction.CollectableAction.Scrutiny],
                        BaseAction.Actions[BaseAction.CollectableAction.Brazen],
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.Progression,
                            Value = 800,
                            ComparisonOperator = Condition.ComparisonOperatorEnum.Lower
                        },
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.CollectorStandard,
                            Value = true
                        }
                    }
                },
                new Turn() // Meticulous if progression >= 800 and Collector's Standard
                {
                    Actions =
                    {
                        BaseAction.Actions[BaseAction.CollectableAction.Meticulous],
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.Progression,
                            Value = 800,
                            ComparisonOperator = Condition.ComparisonOperatorEnum.HigherOrEqual
                        },
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.CollectorStandard,
                            Value = true
                        }
                    }
                },
                new Turn() // Meticulous if progression >= 850
                {
                    Actions =
                    {
                        BaseAction.Actions[BaseAction.CollectableAction.Meticulous],
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.Progression,
                            Value = 850,
                            ComparisonOperator = Condition.ComparisonOperatorEnum.HigherOrEqual
                        }
                    }
                },
                new Turn() // Scour if progression > 800 and no Collector's Standard
                {
                    Actions =
                    {
                        BaseAction.Actions[BaseAction.CollectableAction.Scour],
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.Progression,
                            Value = 800,
                            ComparisonOperator = Condition.ComparisonOperatorEnum.Lower
                        },
                        new Condition
                        {
                            ConditionOn = Condition.ConditionEnum.CollectorStandard,
                            Value = false
                        }
                    }
                }
            }
        };
        var serialize = JsonConvert.SerializeObject(template, JsonSettings);
        var b64 = Convert.ToBase64String(Util.CompressString(serialize));
        Service.Log.Info("Using basic rotation:\n" + serialize + "\n" + b64);
        return Rotation.FromPreset(template, ctx);
    }

    internal static readonly JsonSerializerSettings JsonSettings = new()
    {
        Converters =
        {
            new BaseActionConverter(),
            new StringEnumConverter()
        }
    };
}
