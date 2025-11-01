using LazyGatherer.Solver.Gathering.Models;

namespace LazyGatherer.Solver.Gathering.Comparator
{
    public class GatheringMaxYieldComparer() : RotationComparer(ComparerEnum.MaxYield)
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

            var result = doubleComparer.Compare(x.Yield, y.Yield);

            if (result == 0)
            {
                return -x.UsedGp.CompareTo(y.UsedGp);
            }

            return result;
        }
    }
}
