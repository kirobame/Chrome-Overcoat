using UnityEngine;

namespace Chrome
{
    public interface IIdentity : IRoot
    {
        Faction Faction { get; }

        void Copy(IIdentity identity);
    }
}