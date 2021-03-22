using System;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class TiltState : IData, IWriter<Transform>
    {
        [SerializeField] private CharacterController reference;
        [SerializeField] private SmoothControl rollControl;
        [SerializeField] private SmoothControl pitchControl;

        public void SendDataTo(Transform component)
        {
            var delta = reference.transform.InverseTransformVector(reference.velocity);
            component.localRotation = Quaternion.Euler(pitchControl.Process(delta.y),0.0f,rollControl.Process(delta.x));
        }
    }
}