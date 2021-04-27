using UnityEngine;

namespace Chrome
{
    public class AttackCac : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            Debug.Log("Attack");
            isDone = true;
        }
    }
}
