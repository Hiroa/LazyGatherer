namespace LazyGatherer.Collectable.Actions;

// Récolte
internal class Collect() : CollectableAction(240, 815)
{
    public override bool CanExecute(Rotation rotation)
    {
        return rotation.Context.Attempts > 0 && base.CanExecute(rotation);
    }
}
