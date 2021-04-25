using UnityEngine;

namespace Chrome
{
    public abstract class AltBullet : Bullet
    {
        private ITaskTree taskTree;
        private bool init;

        void Start()
        {
            if (!init) TempInit();
        }
        void OnDestroy() => taskTree.Shutdown(identity.Packet);
        
        private void TempInit()
        {
            identity.Packet.Set(ignores);

            var board = identity.Packet.Get<IBlackboard>();
            board.Set("self", transform);
            
            taskTree = BuildTree();
        }        
        public override void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            identity.Copy(source);

            if (!init) TempInit();
            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
            
            base.Shoot(source, fireAnchor, direction, packet);
        }

        protected override void Update() => taskTree.Update(identity.Packet);

        protected override void OnHit(RaycastHit hit) { }
        protected abstract ITaskTree BuildTree();
    }
}