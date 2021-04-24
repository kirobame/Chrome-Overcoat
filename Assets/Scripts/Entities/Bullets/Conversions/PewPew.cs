using System;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class PewPew : RootNode
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

        public override void Start(Packet packet)
        {
            base.Start(packet);
            if (!init)
            {
                Command(packet, new ChannelRemovalCommand(0b_0001));
                init = true;
            }
        }

        public override IEnumerable<INode> Update(Packet packet)
        {
            switch (step)
            {
                case Step.Move:
                    
                    foreach (var branch in Branches) branch.Update(packet);
                    OnUpdate(packet);

                    if (IsDone)
                    {
                        var board = packet.Get<IBlackboard>();
                        var hit = board.Get<CollisionHit<Transform>>("hit");

                        if (hit != null)
                        {
                            RemoveOutputChannel(packet, 0b_0011);
                            AddOutputChannel(packet, 0b_0100);

                            Start(packet);
                            step = Step.Hit;
                        }
                    }
                    break;
                
                case Step.Hit:
                    
                    foreach (var branch in Branches) branch.Update(packet);
                    OnUpdate(packet);

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
                        
                        Start(packet);
                    }
                    break;
                
                case Step.End:
                    
                    foreach (var branch in Branches) branch.Update(packet);
                    OnUpdate(packet);

                    if (IsDone)
                    {
                        Shutdown(packet);
                        
                        var identity = packet.Get<IIdentity>();
                        identity.Root.gameObject.SetActive(false);
                    }
                    break;
            }

            return null;
        }
    }
}