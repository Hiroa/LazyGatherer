namespace LazyGatherer.Solver.Collectable.Model.Actions;

// Selection
internal class Scrutiny() : BaseAction(22185, 22189)
{
    public override bool CanExecute(Rotation rotation)
    {
        return !rotation.Context.HasScrutiny && base.CanExecute(rotation);
    }
}
