using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunPickup : MonoBehaviour
    {
        public Rigidbody Rigidbody => rigidbody;
        public RetGun Gun => gun;

        [FoldoutGroup("Dependencies"), SerializeField] private new Rigidbody rigidbody;
        
        [FoldoutGroup("Values"), SerializeField] private RetGun gun;

        void Awake() => Events.Subscribe(RetEvent.OnGameEnd, OnGameEnd);
        void OnDestroy() =>  Events.Unsubscribe(RetEvent.OnGameEnd, OnGameEnd);

        void OnGameEnd() => gameObject.SetActive(false);
    }
}