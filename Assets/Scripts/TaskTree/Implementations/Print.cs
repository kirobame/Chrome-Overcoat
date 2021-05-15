using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Print : TaskNode
    {
        public Print(string message) => this.message = message;
        
        [SerializeField] private string message;
        
        protected override void OnUse(Packet packet)
        {
            Debug.Log($"[{Time.frameCount}] {message}");
            isDone = true;
        }
    }
}