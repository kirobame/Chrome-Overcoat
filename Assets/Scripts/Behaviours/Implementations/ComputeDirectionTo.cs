using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class ComputeDirectionTo : ProxyNode
    {
        public ComputeDirectionTo(string path, IValue<Transform> from, IValue<PhysicBody> target)
        {
            this.path = path;

            this.from = from;
            this.target = target;
        }
        
        [SerializeField] private string path;

        private IValue<Transform> from;
        private IValue<PhysicBody> target;

        protected override void OnUpdate(Packet packet)
        {
            if (from.IsValid(packet) && target.IsValid(packet))
            {
                var board = packet.Get<Blackboard>();

                var point = target.Value.transform.position + target.Value.Controller.center;
                board.Set(point - from.Value.position, path);
            }

            IsDone = true;
        }
    }
}