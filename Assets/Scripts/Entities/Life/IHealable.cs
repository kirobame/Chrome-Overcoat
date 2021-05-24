namespace Chrome
{
    public interface IHealable : IInteraction
    {
        void Heal(IIdentity source, float amount, Packet packet);
    }
}