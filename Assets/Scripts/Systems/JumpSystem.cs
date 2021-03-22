using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Order("Update/Jump", "Any/Any")]
    public class JumpSystem : Flux.EDS.System
    {
        public override void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            
            Entities.ForEach((Entity entity, ref PhysicState physics, in ImpulseState impulse) =>
            {
                if (!physics.IsGrounded) return;
                
                physics.velocity += impulse.direction * (impulse.force * Time.fixedDeltaTime);
                Entities.MarkDirty<CharacterController>(entity, physics);

            }, CharacterFlag.Player);
        }
    }
}