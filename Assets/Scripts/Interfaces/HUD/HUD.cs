using UnityEngine;

namespace Chrome
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class HUD : MonoBehaviour
    {
        public RectTransform RectTransform => (RectTransform)transform;
        
        public abstract HUDGroup WantedGroups { get; }
        public HUDBinding HandledBindings => handledBindings;
        
        [SerializeField] private HUDBinding handledBindings;

        public abstract void BindTo(RectTransform frame, IBindable[] bindables);
        public abstract void UnbindFromCurrent();

        public void Discard()
        {
            UnbindFromCurrent();
            gameObject.SetActive(false);
        }
    }
}