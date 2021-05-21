using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class MoveUntil : TaskNode
    {
        public MoveUntil(ICondition condition, float cadence, IValue<NavMeshAgent> navMesh, IValue<Transform> target)
        {
            this.condition = condition;
            this.cadence = new Cadence(cadence);
            
            this.navMesh = navMesh;
            this.target = target;
        }
        public MoveUntil(ICondition condition, float cadence, IValue<NavMeshAgent> navMesh, IValue<Transform> target, IValue<Transform> aim)
        {
            this.condition = condition;
            this.cadence = new Cadence(cadence);
            
            this.navMesh = navMesh;
            this.target = target;
            this.aim = aim;
        }
        
        private ICondition condition;
        private Cadence cadence;
        
        private IValue<NavMeshAgent> navMesh;
        private IValue<Transform> target;
        private IValue<Transform> aim = new EmptyValue<Transform>();

        protected override void OnBootup(Packet packet) => condition.Bootup(packet);
        protected override void Open(Packet packet) => condition.Open(packet);

        protected override void OnPrepare(Packet packet)
        {
            condition.Prepare(packet);
            cadence.Reset();
            
            if (!navMesh.IsValid(packet) || !target.IsValid(packet)) return;
            
            navMesh.Value.updateRotation = true;
            navMesh.Value.isStopped = false;
                
            navMesh.Value.SetDestination(target.Value.position);
            
            if (aim.IsValid(packet)) aim.Value.localRotation = Quaternion.identity;
        }

        protected override void OnUse(Packet packet)
        {
           
            
            if (cadence.Update(Time.deltaTime))
            {
                if (condition.Check(packet)) isDone = true;
                else if (navMesh.IsValid(packet) && target.IsValid(packet)) navMesh.Value.SetDestination(target.Value.position);
            }
        }

        public override void OnClose(Packet packet) => condition.Close(packet);
        protected override void OnShutdown(Packet packet) => condition.Shutdown(packet);
    }
}