using LazyGatherer.Solver.Data;

namespace LazyGatherer.Solver.Comparator
{
    public class GatheringEfficiencyComparer() : RotationComparer("Max yield per GP")
    {
        private readonly DoubleEpsilonComparer doubleComparer = new(1e-6);

        public override int Compare(GatheringOutcome? x, GatheringOutcome? y)
        {
            switch (x)
            {
                case null when y == null:
                    return 0;
                case null:
                    return -1;
            }

            if (y == null)
            {
                return 1;
            }

            var result = doubleComparer.Compare(x.AddYieldPerGp, y.AddYieldPerGp);

            return result == 0 ? doubleComparer.Compare(x.Yield, y.Yield) : result;
        }
    }
}
