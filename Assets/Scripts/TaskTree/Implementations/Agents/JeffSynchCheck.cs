using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class JeffSynchCheck : Condition
    {
        private IValue<Transform> playerTr;

        public override bool Check(Packet packet)
        {
            var root = Refs.ROOT.Reference<Transform>();

            return JeffManager.IsSynchAssaultReady();
        }
    }
}
