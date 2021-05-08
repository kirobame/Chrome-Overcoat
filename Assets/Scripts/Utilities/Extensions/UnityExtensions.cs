using UnityEngine;

namespace Chrome
{
    public static class UnityExtensions
    {
        public static bool HasComponent<T>(this Component component) => component.TryGetComponent<T>(out var discardedOutput);
    }
}