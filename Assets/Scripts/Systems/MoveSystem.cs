using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Order("Update/Move", "Any/Any")]
    public class MoveSystem : Flux.EDS.System
    {
        public override void Update()
        {
            Entities.ForEach((Entity entity, ref MoveState move) =>
            {
                move.direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
                
            }, CharacterFlag.Player);
            
            Entities.ForEach((Entity entity, LookState look, ref PhysicState physics, in MoveState move) =>
            {
                var rotation = Quaternion.AngleAxis(look.yaw, Vector3.up);
                var delta = rotation * move.direction;

                physics.intent += delta.normalized * (move.speed * Time.deltaTime);
            });
        }
    }
}