using UnityEngine;

namespace Chrome
{
    public interface IDamageable
    {
        void Hit(byte ownerType, RaycastHit hit, float damage);
    }
}