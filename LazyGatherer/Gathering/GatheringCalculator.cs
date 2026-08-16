using System;
using System.Collections.Generic;
using System.Linq;
using LazyGatherer.Gathering.Actions;
using LazyGatherer.Gathering.Models;
using MathNet.Numerics.Distributions;

namespace LazyGatherer.Gathering
{
    public static class GatheringCalculator
    {
        public static GatheringOutcome CalculateOutcome(Rotation rotation, GatheringOutcome? baseOutcome = null)
        {
            var context = rotation.Context;
            var outcome = new GatheringOutcome()
            {
                Yield = CalculateYield(context, context.Boon),
                MaxYield = CalculateYield(context, 1, 1),
                MinYield = CalculateYield(context, 0, 0),
                UsedGp = CalculateGp(rotation.Actions)
            };

            if (baseOutcome != null && outcome.UsedGp > 0)
            {
                outcome.AddYieldPerGp = (outcome.Yield - baseOutcome.Yield) / outcome.UsedGp;
            }

            return outcome;
        }

        private static int CalculateGp(IEnumerable<BaseAction> actions)
        {
            return actions.Sum(a => a.Gp);
        }


        private static double CalculateYield(GatheringContext context, double boon, double wiseAttemptProbability = 0.5)
        {
            var wiseBinom = new Binomial(wiseAttemptProbability, context.WiseAttempts);
            var avgYield = 0d;

            for (var i = 0; i <= context.WiseAttempts; i++)
            {
                var wiseSuccessProb = wiseBinom.Probability(i);
                var attempts = context.Attempts + i;

                avgYield += ((context.BaseAmount + boon * (context.HasBoon ? context.BoonBonus : 0)) * attempts +
                             context.BountifulBonus * Math.Min(attempts, context.BountifulAttempts)) * context.Chance *
                            wiseSuccessProb;
            }

            return avgYield;
        }
    }
}
