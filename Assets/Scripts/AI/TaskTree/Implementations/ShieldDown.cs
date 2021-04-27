using UnityEngine;

namespace Chrome
{
    public class ShieldDown : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            Debug.Log("ShieldDown");
            isDone = true;
        }
    }
}
