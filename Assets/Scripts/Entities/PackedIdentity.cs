using UnityEngine;

namespace Chrome
{
    public class PackedIdentity : IExtendedIdentity
    {
        public PackedIdentity(Faction faction, Transform root)
        {
            Faction = faction;
            Root = root;
            
            Board = new Blackboard();
        }
        
        public Faction Faction { get; private set; }
        public Transform Root { get; private set; }
        public IBlackboard Board { get; private set; }

        public void Copy(IIdentity identity) => Faction = identity.Faction;
    }
}