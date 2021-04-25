using UnityEngine;

namespace Chrome
{
    public class GunHUD : MonoBehaviour
    {
        [SerializeField] private GunPartHUD[] subHUDs;

        public void Select(int index)
        {
            if (index < 0 || index >= subHUDs.Length) return;
            
            for (var i = 0; i < index; i++) subHUDs[i].Deactivate();
            for (var i = index + 1; i < subHUDs.Length; i++) subHUDs[i].Deactivate();
            
            subHUDs[index].Activate();
        }
    }
}