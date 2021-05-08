using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetEnemyHealthLink : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private Health health;

        [FoldoutGroup("Values"), SerializeField] private GenericPoolable prefab;

        private bool hasHUD;
        private RetEnemyHealthHUD HUD;
        
        void Awake()
        {
            //hasHUD = false;
            health.onChange += OnHealthChange;
            Events.Subscribe(RetEvent.OnGameEnd, OnGameEnd);
        }
        void OnDestroy()
        {
            health.onChange -= OnHealthChange;
            Events.Unsubscribe(RetEvent.OnGameEnd, OnGameEnd);
        }

        void Update()
        {
            if (!hasHUD) return;
            HUD.transform.position = transform.position;
        }

        public void Bootup()
        {
            var HUDPool = Repository.Get<GenericPool>(RetReference.HUDPool);
            HUD = HUDPool.CastSingle<RetEnemyHealthHUD>();

            var canvas = Repository.Get<Canvas>(RetReference.WorldCanvas);
            HUD.transform.SetParent(canvas.transform);
            HUD.Set(1.0f);

            hasHUD = true;
        }
        public void Shutdown()
        {
            HUD.Set(0.0f);
            HUD.gameObject.SetActive(false);

            hasHUD = false;
        }

        void OnGameEnd()
        {
            if (!hasHUD) return;
            Shutdown();
        }
        void OnHealthChange(float heath, float maxHealth) => HUD.Set(heath / maxHealth);
    }
}