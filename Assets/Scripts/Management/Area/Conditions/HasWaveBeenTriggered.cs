using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class HasWaveBeenTriggered : Condition
    {
        [SerializeField] private string target;

        public override bool Check(Packet packet)
        {
            if (packet.TryGet<WaveControl>(out var waves)) return waves[target].HasBeenTriggered;
            else return false;
        }
    }
}