using System;
using System.Collections.Generic;
using System.Linq;
using LazyGatherer.Solver.Actions;
using LazyGatherer.Solver.Data;
using MathNet.Numerics.Distributions;

namespace LazyGatherer.Solver
{
    public static class GatheringCalculator
    {
        public static GatheringOutcome CalculateOutcome(Rotation rotation, GatheringOutcome? baseOutcome = null)
        {
            var outcome = new GatheringOutcome() 
            { 
                Yield = CalculateYield(rotation.Context),
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
        

        private static double CalculateYield(GatheringContext context)
        {
            var wiseBinom = new Binomial(0.5, context.WiseAttempts);
            var avgYield = 0d;

            for (var i = 0; i <= context.WiseAttempts; i++)
            {
                var wiseSuccessProb = wiseBinom.Probability(i);
                var attempts = context.Attempts + i;
                
                avgYield += ((context.BaseAmount + context.Boon * context.BoonBonus) * attempts + context.BountifulBonus * Math.Min(attempts, context.BountifulAttempts)) * context.Chance * wiseSuccessProb;
                
            }

            return avgYield;
        }
    }
}
