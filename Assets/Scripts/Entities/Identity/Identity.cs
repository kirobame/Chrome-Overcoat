using UnityEngine;
using System.Collections.Generic;

namespace Chrome
{
    public class Identity : Root, IIdentity
    {
        public Faction Faction => faction;

        [SerializeField] private Faction faction;

        protected override void Start()
        {
            base.Start();
            Packet.Set<IIdentity>(this);
        }

        public void Copy(IIdentity identity) => faction = identity.Faction;
    }
}