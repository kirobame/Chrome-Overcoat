using UnityEngine;

namespace Chrome
{
    public interface IIdentity
    {
        Faction Faction { get; }
        Transform Root { get; }
        Packet Packet { get; }
        
        void Copy(IIdentity identity);
    }
}