namespace LazyGatherer.Solver.Collectable.Model.Actions;

// Selection
internal class Scour() : BaseAction(22182, 22186)
{
    public override bool CanExecute(Rotation rotation)
    {
        return rotation.Context.Attempts > 0 && base.CanExecute(rotation);
    }
}
