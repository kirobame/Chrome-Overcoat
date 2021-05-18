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
            if (packet.TryGet<IIdentity>(out var identity)) Debug.Log($"[{Time.frameCount}][{identity.Transform.gameObject.name}] {message}");
            else Debug.Log($"[{Time.frameCount}] {message}");
            
            isDone = true;
        }
    }
}