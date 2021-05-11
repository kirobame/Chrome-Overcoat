using System;
using System.Collections.Generic;
using System.Linq;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class ProjectileNode : RootNode
    {
        #region Nested Types

        private enum Step
        {
            Move,
            Hit,
            End
        }

        #endregion

        private Step step;
        private bool init;

        protected override void OnBootup(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            board.Set("end", false);
            board.Set<CollisionHit<Transform>>("hit", null);
            
            output = 0b_0011;
            init = false;

            step = Step.Move;
        }

        public override void Prepare(Packet packet)
        {
            base.Prepare(packet);
            if (!init)
            {
                Command(packet, new ChannelRemovalCommand(0b_0001));
                init = true;
            }
        }

        public override IEnumerable<INode> Use(Packet packet)
        {
            switch (step)
            {
                case Step.Move:

                    foreach (var branch in Branches) branch.Update(packet);
                    OnUse(packet);

                    if (IsDone)
                    {
                        var board = packet.Get<IBlackboard>();
                        var hit = board.Get<CollisionHit<Transform>>("hit");

                        if (hit != null)
                        {
                            RemoveOutputChannel(packet, 0b_0011);
                            AddOutputChannel(packet, 0b_0100);

                            Prepare(packet);
                            step = Step.Hit;
                            
                            HandleHit(packet);
                        }
                    }
                    break;
                
                case Step.Hit:

                    HandleHit(packet);
                    break;
                
                case Step.End:
                    
                    foreach (var branch in Branches) branch.Update(packet);
                    OnUse(packet);

                    if (IsDone)
                    {
                        Shutdown(packet);
                        
                        var identity = packet.Get<IIdentity>();
                        identity.Transform.gameObject.SetActive(false);
                    }
                    break;
            }

            return null;
        }

        private void HandleHit(Packet packet)
        {
            foreach (var branch in Branches) branch.Update(packet);
            OnUse(packet);
                    
            if (IsDone)
            {
                var board = packet.Get<IBlackboard>();
                var end = board.Get<bool>("end");

                RemoveOutputChannel(packet, 0b_0100);
                if (end)
                {
                    AddOutputChannel(packet, 0b_1000);
                    step = Step.End;
                }
                else
                {
                    board.Set<CollisionHit<Transform>>("hit", null);
                            
                    AddOutputChannel(packet, 0b_0010);
                    step = Step.Move;
                }
                        
                Prepare(packet);
            }
        }
    }
}