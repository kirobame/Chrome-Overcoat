using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "NewCoverProfile", menuName = "Chrome Overcoat/Retro/Cover profile")]
    public class RetCoverProfile : ScriptableObject
    {
        public Vector2 Range => range;
        
        [SerializeField] private Vector2 range;
    }
}