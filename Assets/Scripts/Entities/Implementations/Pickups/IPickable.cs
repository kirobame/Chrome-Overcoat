namespace Chrome
{
    public interface IPickable : ITransform, IInteraction
    {
        void OnHoverStart(IIdentity source);
        void OnHoverEnd(IIdentity source);

        void Pickup(IIdentity source);
    }
}