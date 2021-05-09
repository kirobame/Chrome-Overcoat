using Flux;
using UnityEngine;

namespace Chrome
{
    public abstract class TaskedProjectile : Projectile, IBootable
    {
        private ITaskTree taskTree;
        private bool hasBeenBootedUp;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Start()
        {
            if (hasBeenBootedUp) return;
            
            Bootup();
            hasBeenBootedUp = true;
        }
        void OnDestroy() => taskTree.Shutdown(packet);
        
        public void Bootup()
        {
            packet.Set(ignores);

            var board = packet.Get<IBlackboard>(); 
            board.Set("self", transform);
            
            taskTree = BuildTree();
        }  
        protected abstract ITaskTree BuildTree();
        
        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void OnShoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            base.OnShoot(source, fireAnchor, direction, packet);

            if (!hasBeenBootedUp)
            {
                Bootup();
                hasBeenBootedUp = true;
            }
            taskTree.Bootup(this.packet);
            taskTree.Start(this.packet);
        }

        void Update() => taskTree.Update(packet);
    }
}