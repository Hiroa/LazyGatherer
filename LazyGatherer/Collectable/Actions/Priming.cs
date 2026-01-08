namespace LazyGatherer.Collectable.Actions
{
    // Toucher d'amorçage
    internal class Priming() : CollectableAction(34871, 34872)
    {
        public override bool CanExecute(Rotation rotation)
        {
            return !rotation.Context.HasPriming && base.CanExecute(rotation);
        }
    }
}
