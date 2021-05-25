using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class StompControl : InputControl<StompControl>
    {
        protected override void PrepareInjection() => body = injections.Register(new AnyValue<CharacterBody>());

        protected override void SetupInputs()
        {
            key = new CachedValue<Key>(Key.Inactive);
            input.Value.BindKey(InputRefs.STOMP, this, key);
        }

        //--------------------------------------------------------------------------------------------------------------/

        [FoldoutGroup("Values"), SerializeField] private BindableCooldown cooldown;
        [FoldoutGroup("Values"), SerializeField] private float forward;
        [FoldoutGroup("Values"), SerializeField] private float downward;
        
        private IValue<CharacterBody> body;
        private CachedValue<Key> key;

        private bool state;

        //--------------------------------------------------------------------------------------------------------------/

        void Start()
        {
            state = false;
            HUDBinder.Declare(HUDGroup.Stomp, cooldown);
        }

        void Update()
        {
            if (cooldown.IsActive)
            {
                if (!body.Value.IsGrounded && state) body.Value.velocity += Vector3.down * downward;
                else state = false;
                
                return;
            }
            
            if (body.Value.IsGrounded || !key.IsUp()) return;

            body.Value.velocity = body.Value.transform.forward * forward;
            state = true;
            
            cooldown.Start();
        }
    }
}