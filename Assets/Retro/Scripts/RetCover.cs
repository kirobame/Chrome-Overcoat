using UnityEngine;

namespace Chrome.Retro
{
    public class RetCover : MonoBehaviour
    {
        void Start() => RetCoverSystem.Register(this);
    }
}