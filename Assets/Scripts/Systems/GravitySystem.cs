using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Group("LateUpdate", "Any/End"), Order("LateUpdate/Gravity", "Any/Any")]
    public class GravitySystem : Flux.EDS.System
    {
        public static float strength = 9.81f;
        public static Vector3 direction = Vector3.down;
        
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref PhysicState physic) =>
            {
                if (physic.IsGrounded) physic.velocity = Vector3.zero;

                var ratio = 1.0f - physic.friction;
                physic.velocity += direction * (strength * Time.deltaTime * ratio);
                
                Entities.MarkDirty<CharacterController>(entity, physic);
            });
        }
    }
}