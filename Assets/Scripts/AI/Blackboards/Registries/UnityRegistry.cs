using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class UnityRegistry : MonoBehaviour, IRegistry
    {
        public object RawValue => value;
        [SerializeField] private Object value;
        [SerializeField] private new string name;
        
        public bool IsSource => isSource;
        [Space, SerializeField] private bool isSource;

        [SerializeField, ShowIf("isSource")] private bool isGlobal;

        void Awake()
        {
            if (!isGlobal) return;
            Collect(Blackboard.Global, string.Empty);
        }

        public void Collect(IBlackboard board, string path)
        {
            if (path != string.Empty) path += $".{name}";
            else path = name;
            
            board.Set(path, value);

            if (isSource)
            {
                foreach (var registry in GetComponents<UnityRegistry>())
                {
                    if (registry == this) continue;
                    registry.Collect(board, path);
                }
            }
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                var registries = child.GetComponents<UnityRegistry>();
                if (registries.Any(registry => registry.IsSource)) continue;

                foreach (var registry in registries) registry.Collect(board, path);
            }
        }
    }
}