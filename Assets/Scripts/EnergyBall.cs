using UnityEngine;

namespace Chrome
{
    public class EnergyBall : Bullet
    {
        protected override void OnHit(RaycastHit hit)
        {
            direction = Vector3.Reflect(direction, hit.normal);
            direction.Normalize();
        }
    }
}