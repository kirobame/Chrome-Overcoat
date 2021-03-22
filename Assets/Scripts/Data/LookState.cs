using System;
using System.Collections;
using System.Collections.Generic;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class LookState : IData
    {
        [SerializeField] private Transform yawTarget;
        [SerializeField] private SmoothControl yawControl;
        [HideInInspector] public float yaw;

        [Space, SerializeField] private Transform pitchTarget;
        [SerializeField] private SmoothControl pitchControl;
        [HideInInspector] public float pitch;

        public void Process(Vector2 delta)
        {
            yaw += yawControl.Process(delta.x);
            pitch += pitchControl.Process(delta.y);
        }
        public void Dirty()
        {
            yawTarget.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);
            pitchTarget.localRotation = Quaternion.AngleAxis(pitch, Vector3.right);
        }
    }
}