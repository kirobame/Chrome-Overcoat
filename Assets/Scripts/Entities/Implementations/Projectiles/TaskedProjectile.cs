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
        void OnDestroy() => taskTree.Shutdown(identity.Packet);
        
        public void Bootup()
        {
            identity.Packet.Set(ignores);

            var board = identity.Packet.Get<IBlackboard>(); 
            board.Set("self", transform);
            
            taskTree = BuildTree();
        }  
        protected abstract ITaskTree BuildTree();
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public override void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            identity.Copy(source);

            if (!hasBeenBootedUp)
            {
                Bootup();
                hasBeenBootedUp = true;
            }
            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
            
            base.Shoot(source, fireAnchor, direction, packet);
        }

        void Update() => taskTree.Update(identity.Packet);
    }
}