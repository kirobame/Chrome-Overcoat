using System.Collections;
using System.Collections.Generic;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Group("Update", "Any/LateUpdate"), Order("Update/Camera", "Any/Any")]
    public class CameraSystem : Flux.EDS.System
    {
        public override void Bootup()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        public override void Shutdown()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public override void Update()
        {
            Entities.ForEach((Entity entity, LookState look) =>
            {
                look.Process(new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")));
                look.pitch = Mathf.Clamp(look.pitch, -85.0f, 75.0f);
                look.Dirty();

            }, CharacterFlag.Player);
        }
    }
}