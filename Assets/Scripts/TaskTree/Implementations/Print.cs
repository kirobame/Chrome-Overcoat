using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Print : ProxyNode
    {
        public Print(string message) => this.message = message;
        
        [SerializeField] private string message;
        
        protected override void OnUpdate(Packet packet)
        {
            Debug.Log(message);
            isDone = true;
        }
    }
}