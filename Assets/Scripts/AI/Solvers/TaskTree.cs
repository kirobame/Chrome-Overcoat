using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class TaskTree : Solver
    {
        [SerializeReference] private ITreeBuilder builder;
        
        private ITaskTree tree;

        public override void Build() => tree = builder.Build();

        public override void Bootup()
        {
            var packet = GetPacket();
            tree.Bootup(packet);
            tree.Prepare(packet);
        }

        public override void Evaluate() => tree.Use(GetPacket());

        public override void Shutdown()
        {
            var packet = GetPacket();
            tree.Close(packet);
            tree.Shutdown(packet);
        }

        private Packet GetPacket() => Owner.Identity.Packet;
    }
}