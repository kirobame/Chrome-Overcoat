using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class SimulatedClickInput : RootNode
    {
        public SimulatedClickInput(float duration) => this.duration = duration;
        
        [SerializeField] private float duration;
        
        private float timer;

        protected override void Open(Packet packet) => output = 0b_0001;
        protected override void OnStart(Packet packet) => timer = duration;

        public override IEnumerable<INode> Update(Packet packet)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (!output.HasChannel(0b_0010))
                {
                    Command(packet, new ChannelRemovalCommand(0b_0001));
                    AddOutputChannel(packet, 0b_0010);
                    
                    Start(packet);
                }
                
                if (IsDone) return null;
                
                foreach (var branch in Branches) branch.Update(packet);
                OnUpdate(packet);
                
                if (IsDone) Close(packet);
            }
            else base.Update(packet);
            
            return null;
        }
    }
}