using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class BulletBurst : TaskNode
    {
        public BulletBurst()
        {
        }

        //private int burstAmmount;
        //private int burstAmmountMax;

        protected override void OnUse(Packet packet)
        {
            packet.TryGet<IBlackboard>(out var bb);
            var ammount = bb.Get<int>("weapon.burst");
            ammount--;
            bb.Set<int>("weapon.burst", ammount);
            Debug.Log("BulletBurst" + bb.Get<int>("weapon.burst"));
        }
        /*
        public override bool Check(Packet packet)
        {
            if (burstAmmount <= 0)
            {
                bb.Set<int>("weapon.burst", 0);
                burstAmmount = burstAmmountMax;
                return false;
            }
            else
            {
                burstAmmount--;
                return true;
            }
        }*/
    }
}