using UnityEngine;
using System.Collections.Generic;

namespace Chrome
{
    public class Identity : MonoBehaviour, IIdentity
    {
        public Faction Faction => faction;
        public Transform Root => transform;
        public Packet Packet { get; private set; }
        
        [SerializeField] private Faction faction;

        private List<ILink<IIdentity>> ILinkList = new List<ILink<IIdentity>>();

        void Awake()
        {
            Packet = new Packet();
            
            Packet.Set<IIdentity>(this);
            if (TryGetComponent<IBlackboard>(out var board)) Packet.Set(board);

            GetLinks(this.transform);
            //foreach (var child in GetComponentsInChildren<ILink<IIdentity>>()) child.Link = this;
            foreach (var child in ILinkList) child.Link = this;
        }
        void GetLinks(Transform tr)
        {
            if (tr.GetComponent<Identity>() != null && tr.GetComponent<Identity>() != this) return;

            ILink<IIdentity>[] links = tr.GetComponents<ILink<IIdentity>>();
            if (links.Length > 0)
                foreach (var link in links)
                    if (link != null && !ILinkList.Contains(link))
                        ILinkList.Add(link);

            if (tr.childCount > 0)
                foreach (Transform child in tr)
                    GetLinks(child);
        }


        public void Copy(IIdentity identity) => faction = identity.Faction;
    }
}