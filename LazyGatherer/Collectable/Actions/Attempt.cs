namespace LazyGatherer.Collectable.Actions
{
    // Sagesse du fermier
    internal class Attempt() : CollectableAction(232, 215)
    {
        public override bool CanExecute(Rotation rotation)
        {
            return rotation.Context.Attempts < rotation.Context.MaxAttempts && base.CanExecute(rotation);
        }
    }
}
