using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunModel : MonoBehaviour
    {
        public Transform FireAnchor => fireAnchor;
        
        [FoldoutGroup("Dependencies"), SerializeField] private Transform fireAnchor;
    }
}