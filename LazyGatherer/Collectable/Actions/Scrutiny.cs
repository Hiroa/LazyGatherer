namespace LazyGatherer.Collectable.Actions;

// Inspection rigoureuse
internal class Scrutiny() : CollectableAction(22185, 22189)
{
    public override bool CanExecute(Rotation rotation)
    {
        return !rotation.Context.HasScrutiny && base.CanExecute(rotation);
    }
}
