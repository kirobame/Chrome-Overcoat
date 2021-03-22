using System;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct ImpulseState : IData
    {
        public Vector3 direction;
        public float force;
    }
}