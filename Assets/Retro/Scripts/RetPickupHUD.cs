using Flux.Event;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome.Retro
{
    public class RetPickupHUD : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private EventHandler handler;
        [FoldoutGroup("Dependencies"), SerializeField] private CanvasGroup group;
        [FoldoutGroup("Dependencies"), SerializeField] private Image icon;
        [FoldoutGroup("Dependencies"), SerializeField] private TMP_Text ammo;
        [FoldoutGroup("Dependencies"), SerializeField] private TMP_Text title;

        void Awake()
        {
            handler.AddDependency(Events.Subscribe<RetGunPickup>(RetEvent.OnGunFound, OnGunFound));
            handler.AddDependency(Events.Subscribe(RetEvent.OnGunLost, OnGunLost));
        }
        
        void OnGunFound(RetGunPickup pickup)
        {
            icon.sprite = pickup.Gun.Icon;
            icon.SetNativeSize();
            
            ammo.text = pickup.ammo < 10 ? $"0{pickup.ammo}" : pickup.ammo.ToString();
            title.text = pickup.Gun.Title;
            
            group.alpha = 1.0f;
        }

        void OnGunLost() => group.alpha = 0.0f;
    }
}