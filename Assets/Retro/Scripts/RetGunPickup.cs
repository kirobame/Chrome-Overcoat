using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunPickup : MonoBehaviour
    {
        public RetGun Gun => gun;
        
        [FoldoutGroup("Values"), SerializeField] private RetGun gun;
    }
}