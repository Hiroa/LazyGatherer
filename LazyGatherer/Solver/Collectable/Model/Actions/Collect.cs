namespace LazyGatherer.Solver.Collectable.Model.Actions;

// Récolte
internal class Collect() : BaseAction(240, 815)
{
    public override bool CanExecute(Rotation rotation)
    {
        return rotation.Context.Attempts > 0 && base.CanExecute(rotation);
    }
}
