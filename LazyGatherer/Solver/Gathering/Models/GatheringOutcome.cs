namespace LazyGatherer.Solver.Gathering.Models
{
    public class GatheringOutcome
    {
        public int UsedGp { get; init; }
        public double Yield { get; init; }
        public double AddYieldPerGp { get; set; }

        public double MaxYield { get; set; }

        public double MinYield { get; set; }
    }
}
