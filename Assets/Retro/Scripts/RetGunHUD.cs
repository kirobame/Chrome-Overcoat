using Flux.Event;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome.Retro
{
    public class RetGunHUD : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private EventHandler handler;
        [FoldoutGroup("Dependencies"), SerializeField] private Image icon;
        [FoldoutGroup("Dependencies"), SerializeField] private TMP_Text ammo;
        
        void Awake()
        {
            handler.AddDependency(Events.Subscribe<RetGun,int>(RetEvent.OnGunSwitch, OnGunSwitch));
            handler.AddDependency(Events.Subscribe<int>(RetEvent.OnAmmoChange, OnAmmoChange));
        }

        void OnGunSwitch(RetGun gun, int ammo)
        {
            icon.sprite = gun.Icon;
            OnAmmoChange(ammo);
        }

        void OnAmmoChange(int ammo)
        {
            if (ammo < 0) this.ammo.enabled = false;
            else
            {
                this.ammo.enabled = true;
                this.ammo.text = ammo < 10 ? $"0{ammo}" : ammo.ToString();
            }
        }
    }
}