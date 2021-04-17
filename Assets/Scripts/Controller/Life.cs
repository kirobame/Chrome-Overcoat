using UnityEngine;

namespace Chrome
{
    public class Life : MonoBehaviour, IDamageable
    {
        public void Hit(RaycastHit hit, float damage) { }
    }
}