namespace LazyGatherer.Solver.Collectable.Model.Actions;

// Selection méthodique
internal class Meticulous() : BaseAction(22184, 22188)
{
    public override bool CanExecute(Rotation rotation)
    {
        return rotation.Context.Attempts > 0 && base.CanExecute(rotation);
    }
}
