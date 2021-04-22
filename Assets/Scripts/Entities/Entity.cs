using UnityEngine;

namespace Chrome
{
    public class Entity : MonoBehaviour, IExtendedIdentity
    {
        public Faction Faction => faction;
        public Transform Root => transform;
        public IBlackboard Board => board;
        
        [SerializeField] private Faction faction;
        [SerializeField] private ConcreteBoard board;
        
        public void Copy(IIdentity identity) => faction = identity.Faction;
    }
}