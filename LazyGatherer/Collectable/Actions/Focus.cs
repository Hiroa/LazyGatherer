namespace LazyGatherer.Collectable.Actions
{
    // Oeil du collectionneur
    internal class Focus() : CollectableAction(21205, 21206)
    {
        public override bool CanExecute(Rotation rotation)
        {
            return !rotation.Context.HasFocus && base.CanExecute(rotation);
        }
    }
}
