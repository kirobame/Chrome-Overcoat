using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private InputActionAsset inputs;

        private InputAction moveAction;

        private Vector3 moveIntent;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
    
        private float playerSpeed = 2.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;

        void Awake()
        {
            inputs.Enable();
        
            moveAction = inputs.FindAction("Gameplay/Move");
            moveAction.performed += Move;
            moveAction.canceled += EndMove;
        }
        void OnDestroy()
        {
            moveAction.performed -= Move;
            moveAction.canceled -= EndMove;
        
            inputs.Disable();
        }

        private void Move(InputAction.CallbackContext ctxt)
        {
            var value = ctxt.ReadValue<Vector2>();
            moveIntent = new Vector3(value.x, 0, value.y);
        }
        private void EndMove(InputAction.CallbackContext ctxt) => moveIntent = Vector3.zero;
    
        void Update()
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0) playerVelocity.y = 0f;

            controller.Move(moveIntent * (Time.deltaTime * playerSpeed));

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
}