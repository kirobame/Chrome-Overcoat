using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CoverSpot
    {
        public CoverSpot(Vector3 position, Vector3 orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }
        
        public Area Area { get; set; }

        public Vector3 Position => position;
        public Vector3 Orientation => orientation;
        
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 orientation;
    }
}