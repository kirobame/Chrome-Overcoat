namespace Chrome
{
    public interface IPickable : ITransform, IInteraction
    {
        float Radius { get; }
        
        void OnHoverStart(IIdentity source);
        void OnHoverEnd(IIdentity source);

        void Pickup(IIdentity source);
    }
}