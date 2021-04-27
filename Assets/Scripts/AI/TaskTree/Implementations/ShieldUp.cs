using UnityEngine;

namespace Chrome
{
    public class ShieldUp : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            Debug.Log("ShieldUp");
            isDone = true;
        }
    }
}
