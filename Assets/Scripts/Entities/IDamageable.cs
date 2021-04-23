using System.Collections.Generic;

namespace Chrome
{
    public interface IDamageable : IInteraction
    {
        IIdentity Identity { get; }
        
        void Hit(IIdentity source, float amount, Packet packet);
    }
}