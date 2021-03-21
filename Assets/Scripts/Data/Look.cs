using System;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct Look : IData, IWriter<Transform>
    {
        public Vector3 Rotation
        {
            get => new Vector3(pitch, yaw, roll);
            set
            {
                pitch = value.x;
                yaw = value.y;
                roll = value.z;
            }
        }
    
        public float pitch;
        public float yaw;
        public float roll;
    
        public void SendDataTo(Transform component) => component.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }
}