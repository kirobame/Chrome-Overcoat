using UnityEngine;

namespace Chrome
{
    public class Identity : MonoBehaviour, IIdentity
    {
        public Faction Faction => faction;
        public Transform Root => transform;
        public Packet Packet { get; private set; }
        
        [SerializeField] private Faction faction;

        void Awake()
        {
            Packet = new Packet();
            
            Packet.Set<IIdentity>(this);
            if (TryGetComponent<IBlackboard>(out var board)) Packet.Set(board);

            foreach (var child in GetComponentsInChildren<ILink<IIdentity>>()) child.Link = this;
        }
        
        public void Copy(IIdentity identity) => faction = identity.Faction;
    }
}