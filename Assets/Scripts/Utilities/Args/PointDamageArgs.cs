using System;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class PointDamageArgs : EventArgs, IWrapper<float>
    {
        public PointDamageArgs(RaycastHit hit, float amount)
        {
            Hit = hit;
            Amount = amount;
        }
        
        float IWrapper<float>.Value => Amount;

        public RaycastHit Hit { get; private set; }
        public float Amount { get; private set; }
    }
}