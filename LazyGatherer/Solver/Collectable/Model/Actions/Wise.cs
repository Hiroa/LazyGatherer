namespace LazyGatherer.Solver.Collectable.Model.Actions
{
    // Etalage de conaissance
    internal class Wise() : BaseAction(26521, 26522)
    {
        public override bool CanExecute(Rotation rotation)
        {
            return rotation.Context.Attempts < rotation.Context.MaxAttempts
                   && rotation.Context.HasEureka
                   && base.CanExecute(rotation);
        }
    }
}
