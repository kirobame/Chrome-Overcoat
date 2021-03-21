using Flux.EDS;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    [Group("Update", "Any/Any"), Order("Update/Controller", "Any/Any")]
    public class Controller : BindedSystem
    {
        private InputActionAsset inputs;
        private PlayerSettings settings;

        private InputAction mouseMoveAction;
        private Vector2 mouseDelta;
        private Vector2 lookVelocity;

        private InputAction moveAction;
        private Vector2 moveDelta;

        public override void Initialize()
        {
            base.Initialize();
            
            BindWhole<InputActionAsset>("Core/Inputs", inputs => this.inputs = inputs);
            BindWhole<PlayerSettings>("Gameplay/Settings/Player", settings => this.settings = settings);
        }

        protected override void OnBindingsResolved()
        {
            inputs.Enable();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            mouseMoveAction = inputs.FindAction("Gameplay/Look");
            mouseMoveAction.performed += OnMouseMove;
            mouseMoveAction.canceled += OnMouseMoveEnd;

            moveAction = inputs.FindAction("Gameplay/Move");
            moveAction.performed += OnMove;
            moveAction.canceled += OnMoveEnd;
        }
        public override void Shutdown()
        {
            mouseMoveAction.performed -= OnMouseMove;
            mouseMoveAction.canceled -= OnMouseMoveEnd;
            
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMoveEnd;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            inputs.Disable();
        }

        public override void Update()
        {
            Entities.ForEach((Entity entity, ref Look look) =>
            {
                var rotation = settings.ComputeRotation(mouseDelta, look, ref lookVelocity);
                look.pitch = rotation.y;
                look.yaw = rotation.x;
                    
                Entities.MarkDirty<Transform>(entity, look);

            }, Character.Player);

            Entities.ForEach((Entity entity, ref Move move) =>
            {
                move.velocity = settings.ComputeMovement(moveDelta);

            }, Character.Player);
        }

        void OnMouseMove(InputAction.CallbackContext ctxt)
        {
            mouseDelta = ctxt.ReadValue<Vector2>();
            mouseDelta.y *= -1;
        }
        void OnMouseMoveEnd(InputAction.CallbackContext ctxt) => mouseDelta = Vector2.zero;

        void OnMove(InputAction.CallbackContext ctxt) => moveDelta = ctxt.ReadValue<Vector2>();
        void OnMoveEnd(InputAction.CallbackContext ctxt) => moveDelta = Vector2.zero;
    }
}