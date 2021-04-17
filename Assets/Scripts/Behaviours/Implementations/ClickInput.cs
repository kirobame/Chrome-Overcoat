using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class ClickInput : RootNode
    {
        public ClickInput(float duration) => this.duration = duration;
        
        [SerializeField] private float duration;
        
        private float timer;

        protected override void OnStart(Packet packet)
        {
            timer = duration;
            output = 0b_0001;
        }

        public override IEnumerable<Node> Update(Packet packet)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                output = 0b_0010;
                
                OnUpdate(packet);
                UpdateCachedNodes(packet);
                
                if (CanBreak())
                {
                    Shutdown();
                    return Array.Empty<Node>();
                }
            }
            else base.Update(packet);
            
            return null;
        }
    }
}