using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "NewCoverProfile", menuName = "Chrome Overcoat/Retro/Cover profile")]
    public class RetCoverProfile : ScriptableObject
    {
        public Vector2 Range => range;
        public float MaxFlank => maxFlank;
        public Vector2 Distances => distances;
        
        [SerializeField] private Vector2 range;
        [SerializeField, Range(-1.0f, 1.0f)] private float maxFlank;
        [SerializeField] private Vector2 distances;
    }
}