using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class StrafAround : TaskNode
    {
        public StrafAround(IValue<NavMeshAgent> navMesh, IValue<Transform> aim, IValue<Transform> selfRoot, IValue<Transform> playerRoot)
        {
            this.navMesh = navMesh;
            this.aim = aim;
            this.playerRoot = playerRoot;
            this.selfRoot = selfRoot;
        }
        
        private IValue<NavMeshAgent> navMesh;
        private float toRight;
        private IValue<Transform> aim;// = new EmptyValue<Transform>();
        private IValue<Transform> playerRoot;
        private IValue<Transform> selfRoot;

        //new StrafAround(navRef, pivotRef, JeffManager.GetSide2D(rootRef.Value, JeffManager.GetClosestJeff(rootRef.Value))),
        protected override void OnUse(Packet packet)
        {
            if (navMesh.IsValid(packet) && aim.IsValid(packet) && selfRoot.IsValid(packet))
            {
                toRight = JeffManager.GetSide(selfRoot.Value, JeffManager.GetClosestJeff(selfRoot.Value), aim.Value);
                //Debug.Log(toRight);
                navMesh.Value.updateRotation = false;
                navMesh.Value.isStopped = false;
                //Debug.Log(aim.Value.forward);

                if(toRight > 0)
                {
                    var direction = Quaternion.Euler(0, 90, 0) * aim.Value.forward;
                    navMesh.Value.SetDestination(navMesh.Value.transform.position + direction);
                }
                else if (toRight < 0)
                {
                    var direction = Quaternion.Euler(0, -90, 0) * aim.Value.forward;
                    navMesh.Value.SetDestination(navMesh.Value.transform.position + direction);
                }
                else
                {
                    navMesh.Value.isStopped = true;
                }
                //navMesh.Value.transform.Translate(direction, Space.Self);


                
                //if (aim.IsValid(packet)) aim.Value.localRotation = Quaternion.identity;
            }
            
            isDone = true;
        }
    }
}