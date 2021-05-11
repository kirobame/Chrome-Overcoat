using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Print : TaskedNode
    {
        public Print(string message) => this.message = message;
        
        [SerializeField] private string message;
        
        protected override void OnUse(Packet packet)
        {
            Debug.Log(message);
            isDone = true;
        }
    }
}