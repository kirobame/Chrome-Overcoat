using System;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct MoveState : IData
    {
        [HideInInspector] public Vector3 direction;
        public float speed;
    }
}