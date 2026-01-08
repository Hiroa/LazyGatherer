using System.Collections.Generic;
using LazyGatherer.Collectable.Conditions;
using static LazyGatherer.Collectable.Actions.CollectableAction;

namespace LazyGatherer.Collectable.Presets;

/**
 * Built-in presets for collectables
 * Every preset are made from TC guides available at https://guides.ffxivteamcraft.com/guide/gathering-collectable-guide
 * TC are not affiliated with LazyGatherer in any way.
 */
internal static class BuiltIn
{
    internal static readonly Dictionary<string, Preset> Presets = new()
    {
        { "1000/700GP", Preset.Import(ThousandGpString) },
        { "400/600GP Low stats", Preset.Import(FourHundredGpLowStatString) },
        { "0GP Ephemeral", Preset.Import(ZeroGpEphemeralString) },
    };

    /**
     * Built-in 1000/700GP preset
     * 1000 and 700 GP are basically the same rotation
     * Just an extra attempt at the end and Brazen instead of Meticulous Scour if you have Collector's Standard while under 800 progression.
     */
    internal const string ThousandGpString =
        "H4sIAAAAAAAACsWSsWrDQAyGX8VovsEdCsVbEoqXtgmx6VI8iDu1MZzvXJ1uSIzfvZg0pK1D6kJDNg0S36df6uAJG4IMyo2PAZ1J8lVSLpK1F5TaO1CwJu2bhpwhk7eQ3aRpqqAQagNkLx3M9NA31DAToaYVULDw1pIWqIbSmfrQ0sGyJUbxDBnkTCjE5Qbdku/fI1pQUG7bQWfF/o0phL3CM9pIe3Rf9eobdBLqgUI4yTkqHxgjQKE5Su22oGDOuCM3DfXLLndpOnC+DP4w+9zLcyHoDLI5zgpHOqf5SFLraH0MV1d9RRvGrlMF//wh/57q5Uxvx69caB/54olMvfoJwTNpVH3VfwAetLXYTQQAAA==";

    /**
     * Built-in 0GP Ephemeral preset
     * Better to use on Ephemeral if don't have enough GP for the 1000/700GP preset
     */
    internal const string FourHundredGpLowStatString =
        "H4sIAAAAAAAACpXPQUsDMRQE4L9S5hwwQvGwt1JkL7UtbvEiewjZhwayefHlBSnL/ndZsAhWCt4G5vDNTNi7kdBgbe3dg7XtcbXjz1WnTmHwTJ7HkdJAQ5vRrK016JRyQfM6YeM1cFoythwjeUVvsOU0hEsx4ZBJnLKgQSvklOT07tJBHj+qizA4nfPCH4XfhEoJnGDw4mIlNPfW2rmfzf+pHZXyp7NRpTEv3y7GFdB5qRrSGQZPpMHXyLXc5n4h3xNZOnVpcDL8cCqVbomd5ypXWD/38xdhC4ktqgEAAA==\n";

    /**
     * Built-in 400/600GP Low Stat preset
     * Better to use on characters with low gathering stats
     */
    internal const string ZeroGpEphemeralString =
        "H4sIAAAAAAAACq3QwUrEQAwG4FcpOc+hggeZ21KkF3WXbfGg9BBmghamkzGTOUjpu0tRWXQLKngLJD9fkhnucCKw8EDCVXuo+qY6sqKOHMHAkRxPE0VPvk1gawOdUspgH2fYuXVoraHhEMgpDAYajn78bMywTySoLGChFUIl6Z8x7uX6pWAAA/1rWvWD8JNQzu/qPYZCYC/rehkW83fphnLeZHaqNCU9ERdnwC3p6Ergkn9nwM83mC/Bbyt9HMTSKUaP4k9ZlUJn+3WOi/zzo682Hr3tDMuwvAGFewEdMwIAAA==";

#if DEBUG
    public static Preset ThousandGp()
    {
        return new Preset
        {
            Name = "1000/700GP",
            RecommendedGp = 1000,
            Steps =
            {
                // If progression >= 1000, Attempt and Collect
                new Step
                {
                    Actions =
                    {
                        Names.Attempt,
                        Names.Collect
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.GreaterThanOrEqual,
                            Value = 1000
                        }
                    }
                },
                // If attempts <= 1, Collect
                new Step
                {
                    Actions =
                    {
                        Names.Collect
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Attempt,
                            Operator = Operator.LessThanOrEqual,
                            Value = 1
                        }
                    }
                },
                // If Progression < 800 and Collector Standard is active, Scrutiny and Brazen
                new Step
                {
                    Actions =
                    {
                        Names.Scrutiny,
                        Names.Brazen
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.LessThan,
                            Value = 800
                        },
                        new Condition
                        {
                            Type = Type.CollectorStandard,
                            Operator = Operator.Equal,
                            Value = true
                        }
                    }
                },
                // If Progression < 800 and Collector Standard is not active, Scrutiny and Meticulous
                new Step
                {
                    Actions =
                    {
                        Names.Scrutiny,
                        Names.Meticulous
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.LessThan,
                            Value = 800
                        },
                        new Condition
                        {
                            Type = Type.CollectorStandard,
                            Operator = Operator.Equal,
                            Value = false
                        }
                    }
                },
                // if Progression >= 800 and Collector Standard is active, Meticulous
                new Step
                {
                    Actions =
                    {
                        Names.Meticulous
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.GreaterThanOrEqual,
                            Value = 800
                        },
                        new Condition
                        {
                            Type = Type.CollectorStandard,
                            Operator = Operator.Equal,
                            Value = true
                        }
                    }
                },
                // if Progression >= 850, Meticulous
                new Step
                {
                    Actions =
                    {
                        Names.Meticulous
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.GreaterThanOrEqual,
                            Value = 850
                        }
                    }
                },
                // if progression >= 800 and < 850, Scour
                new Step
                {
                    Actions =
                    {
                        Names.Scour
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.GreaterThanOrEqual,
                            Value = 800
                        },
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.LessThan,
                            Value = 850
                        }
                    }
                },
                // Default Step: Meticulous
                new Step
                {
                    Actions =
                    {
                        Names.Meticulous
                    }
                }
            }
        };
    }

    public static Preset ZeroGpEphemeral()
    {
        return new Preset
        {
            Name = "0GP Ephemeral",
            RecommendedGp = 0,
            Steps =
            {
                // If progression >= 400, Collect
                new Step
                {
                    Actions =
                    {
                        Names.Collect
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.GreaterThanOrEqual,
                            Value = 400
                        }
                    }
                },
                // If attempts <= 1, Collect
                new Step
                {
                    Actions =
                    {
                        Names.Collect
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Attempt,
                            Operator = Operator.LessThanOrEqual,
                            Value = 1
                        }
                    }
                },
                // If Progression < 400 and Collector Standard is active, Meticulous
                new Step
                {
                    Actions =
                    {
                        Names.Meticulous
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.LessThan,
                            Value = 400
                        },
                        new Condition
                        {
                            Type = Type.CollectorStandard,
                            Operator = Operator.Equal,
                            Value = true
                        }
                    }
                },
                // if Progression < 400, Scour
                new Step
                {
                    Actions =
                    {
                        Names.Scour
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.GreaterThanOrEqual,
                            Value = 800
                        }
                    }
                },
                // Default Step: Scour
                new Step
                {
                    Actions =
                    {
                        Names.Scour
                    }
                }
            }
        };
    }

    public static Preset FourHundredLowStat()
    {
        return new Preset()
        {
            Name = "400/600GP Low Stat",
            RecommendedGp = 400,
            Steps =
            {
                // If progression >= 1000, Collect
                new Step
                {
                    Actions =
                    {
                        Names.Collect
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Progression,
                            Operator = Operator.GreaterThanOrEqual,
                            Value = 1000
                        }
                    }
                },
                // If attempts <= 1, Collect
                new Step
                {
                    Actions =
                    {
                        Names.Collect
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.Attempt,
                            Operator = Operator.LessThanOrEqual,
                            Value = 1
                        }
                    }
                },
                // If Collector Standard is active, Scrutiny and Meticulous
                new Step
                {
                    Actions =
                    {
                        Names.Scrutiny,
                        Names.Meticulous
                    },
                    Conditions =
                    {
                        new Condition
                        {
                            Type = Type.CollectorStandard,
                            Operator = Operator.Equal,
                            Value = true
                        }
                    }
                },
                // Default Step: Scrutiny and Meticulous
                new Step
                {
                    Actions =
                    {
                        Names.Scrutiny,
                        Names.Scour
                    }
                }
            }
        };
    }
#endif
}
