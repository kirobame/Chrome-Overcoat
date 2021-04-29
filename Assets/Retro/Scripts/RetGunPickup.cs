using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunPickup : MonoBehaviour, IBootable
    {
        public Rigidbody Rigidbody => rigidbody;
        public RetGun Gun => gun;

        [FoldoutGroup("Dependencies"), SerializeField] private new Rigidbody rigidbody;
        
        [FoldoutGroup("Values"), SerializeField] private RetGun gun;

        [ReadOnly, HideInEditorMode] public int ammo;

        void Awake() => Events.Subscribe(RetEvent.OnGameEnd, OnGameEnd);
        void OnDestroy() =>  Events.Unsubscribe(RetEvent.OnGameEnd, OnGameEnd);

        void OnGameEnd()
        {
            if (!gameObject.activeInHierarchy) return;
            gameObject.SetActive(false);
        }

        public void Bootup() => ammo = gun.MaxAmmo;
    }
}