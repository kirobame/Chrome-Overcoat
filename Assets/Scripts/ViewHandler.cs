﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class ViewHandler : MonoBehaviour
    {
        [BoxGroup("Yaw"), SerializeField] private Transform yawTarget;
        [BoxGroup("Yaw"), SerializeReference] private IMouseHandler yaw;

        [BoxGroup("Pitch"), SerializeField] private Transform pitchTarget;
        [BoxGroup("Pitch"), SerializeReference] private IMouseHandler pitch;
        
        void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            yaw.Bootup(yawTarget.localEulerAngles.y);
            pitch.Bootup(pitchTarget.localEulerAngles.x);
        }
        void OnDestroy()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        void Update()
        {
            var eulerAngles = yawTarget.localEulerAngles;
            eulerAngles.y = yaw.Process(Input.GetAxisRaw("Mouse X"));
            yawTarget.localEulerAngles = eulerAngles;

            eulerAngles = pitchTarget.localEulerAngles;
            eulerAngles.x = pitch.Process(-Input.GetAxisRaw("Mouse Y"));
            pitchTarget.localEulerAngles = eulerAngles;
        }
    }
}