using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class TakeCover : TaskNode
    {
        public TakeCover(float acceptance, float range, float errorThreshold, IValue<AreaLink> link, IValue<NavMeshAgent> nav, IValue<Transform> view, IValue<Collider> target)
        {
            this.acceptance = acceptance;
            this.range = range;
            this.errorThreshold = errorThreshold;
            
            this.link = link;
            this.nav = nav;
            this.view = view;
            this.target = target;
        }

        private float acceptance;
        private float range;
        private float errorThreshold;
        
        private IValue<AreaLink> link;
        private IValue<NavMeshAgent> nav;
        private IValue<Transform> view;
        private IValue<Collider> target;
        
        private bool hasCover;
        private float error;
        
        protected override void OnPrepare(Packet packet)
        {
            if (!link.IsValid(packet) || !nav.IsValid(packet) || !view.IsValid(packet) || !target.IsValid(packet))
            {
                hasCover = false;
                return;
            }

            error = 0.0f;

            var position = nav.Value.transform.position;
            var offset = view.Value.position - position;

            var board = packet.Get<IBlackboard>();
            if (board.TryGet<CoverSpot>(AgentRefs.COVER, out var previousCover) && previousCover != null)
            {
                hasCover = true;
                return;
            }
            
            if (CoverSystem.Request(link.Value.Area, position, offset, range, acceptance, target.Value, out var availableCover))
            {
                nav.Value.updateRotation = true;
                nav.Value.isStopped = false;
                nav.Value.SetDestination(availableCover.Position);
                
                if (view.IsValid(packet)) view.Value.localRotation = Quaternion.identity;
                
                board.Set<CoverSpot>(AgentRefs.COVER, availableCover);
                hasCover = true;
            }
            else
            {
                board.Set<CoverSpot>(AgentRefs.COVER, null);
                hasCover = false;
            }
        }

        protected override void OnUse(Packet packet)
        {
            if (!link.IsValid(packet) || !nav.IsValid(packet) || !view.IsValid(packet) || !target.IsValid(packet))
            {
                isDone = true;
                return;
            }
            
            if (!hasCover)
            {
                isDone = true;
                return;
            }

            if (!nav.Value.pathPending && nav.Value.remainingDistance <= nav.Value.stoppingDistance + 0.1f) isDone = true;
            else
            {
                if (!view.Value.position.CanSee(target.Value, LayerMask.GetMask("Environment")))
                {
                    error += Time.deltaTime;
                    if (error >= errorThreshold)
                    {
                        DiscardCover(packet);
                        isDone = true;
                    }
                }
                else
                {
                    error -= Time.deltaTime;
                    if (error < 0.0f) error = 0.0f;
                }
            }
        }

        protected override void OnShutdown(Packet packet)
        {
            if (!hasCover) return;
            
            if (nav.IsValid(packet)) nav.Value.ResetPath();
            DiscardCover(packet);
        }

        private void DiscardCover(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            CoverSystem.Discard(board.Get<CoverSpot>(AgentRefs.COVER));
            
            board.Set<CoverSpot>(AgentRefs.COVER, null);
        }
    }
}