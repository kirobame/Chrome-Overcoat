using UnityEngine;

namespace Chrome.Retro
{
    public class RetCover : MonoBehaviour
    {
        void Awake() => RetCoverSystem.Register(this);
    }
}