using System;
using UnityEngine;

namespace Flux
{
    public class Hook : MonoBehaviour
    {
        public event Action onDestroyed;

        void OnDestroy() => onDestroyed?.Invoke();
    }
}