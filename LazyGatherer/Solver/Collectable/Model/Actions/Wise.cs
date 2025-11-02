namespace LazyGatherer.Solver.Collectable.Model.Actions
{
    // Sagesse du fermier
    internal class Wise() : BaseAction(26521, 26522)
    {
        public override bool CanExecute(Rotation rotation)
        {
            return rotation.Context.Attempts < rotation.Context.MaxAttempts && base.CanExecute(rotation);
        }
    }
}
