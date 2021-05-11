using System;
using Flux;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class PulseCommand : Command
    {
        public PulseCommand(INode target) : base(new OnRootFree()) => this.target = target;
        
        public INode Target => target;
        [SerializeField] private INode target;

        public override void Execute(Packet packet, ITaskTree source)
        {
            if (source.IsUpdating(target))
            {
                target.Prepare(packet);
                return;
            }
                    
            if (!target.IsChildOf(source)) return;

            Branch match = null;
            foreach (var branch in source.Branches)
            {
                for (var i = 0; i < branch.Nodes.Count; i++)
                {
                    if (!branch.Nodes[i].IsChildOf(target)) continue;

                    branch.RemoveAt(i);
                    i--;

                    match = branch;
                }
            }
            
            if (match != null)
            {
                target.Prepare(packet);
                match.Add(target);
            }
        }
    }
}