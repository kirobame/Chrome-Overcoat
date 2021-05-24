using TMPro;
using UnityEngine;

namespace Chrome
{
    public class PickupHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Pickup;

        [SerializeField] private TMP_Text info;

        private BindableState<IPickable> pickableBinding;

        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);
            
            pickableBinding = bindables.Get<BindableState<IPickable>>(HUDBinding.Popup);
            pickableBinding.onChange += OnPickableChange;
        }
        public override void UnbindFromCurrent() => pickableBinding.onChange -= OnPickableChange;

        //--------------------------------------------------------------------------------------------------------------/

        void OnPickableChange(IPickable pickable)
        {
            if (pickableBinding.HasValue && pickable is IPopupInfo popup)
            {
                info.enabled = true;
                info.text = popup.Text;
            }
            else info.enabled = false;
        }
    }
}