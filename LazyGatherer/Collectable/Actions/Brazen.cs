namespace LazyGatherer.Collectable.Actions;

// Selection instinctive
internal class Brazen() : CollectableAction(22183u, 22187u)
{
    public override bool CanExecute(Rotation rotation)
    {
        return rotation.Context.Attempts > 0 && base.CanExecute(rotation);
    }
}
