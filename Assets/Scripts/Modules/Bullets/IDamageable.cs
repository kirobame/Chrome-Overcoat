using UnityEngine;

namespace Chrome
{
    public interface IDamageable
    {
        void Hit(RaycastHit hit, float damage);
    }
}