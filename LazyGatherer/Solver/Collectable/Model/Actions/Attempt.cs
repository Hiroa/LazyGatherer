namespace LazyGatherer.Solver.Collectable.Model.Actions
{
    // Sagesse du fermier
    internal class Attempt() : BaseAction(232, 215)
    {
        public override bool CanExecute(Rotation rotation)
        {
            return rotation.Context.Attempts < rotation.Context.MaxAttempts && base.CanExecute(rotation);
        }
    }
}
