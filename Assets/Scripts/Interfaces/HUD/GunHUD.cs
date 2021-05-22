using UnityEngine;

namespace Chrome
{
    public class GunHUD : MonoBehaviour, IDeprecatedHUD
    {
        [SerializeField] private GunPartHUD[] subHUDs;

        public void Refresh(object value, string tag)
        {
            switch (tag)
            {
                default:
                    Select((int)value);
                    break;
            }
        }
        private void Select(int index)
        {
            if (index < 0 || index >= subHUDs.Length) return;
            
            for (var i = 0; i < index; i++) subHUDs[i].Deactivate();
            for (var i = index + 1; i < subHUDs.Length; i++) subHUDs[i].Deactivate();
            
            subHUDs[index].Activate();
        }
    }
}