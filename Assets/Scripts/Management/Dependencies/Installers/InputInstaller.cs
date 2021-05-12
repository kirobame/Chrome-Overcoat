using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class InputInstaller : MonoBehaviour
    {
        [FoldoutGroup("Values"), SerializeField] private InputActionAsset asset;

        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference view;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference jump;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference move;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference sprint;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference shoot;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference cast;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference pickWeapon01;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference pickWeapon02;
        
        private Blackboard blackboard;
        
        void Awake()
        {
            blackboard = new Blackboard();
            Blackboard.Global.Set<IBlackboard>(InputRefs.BOARD, blackboard);

            var gameplayMap = asset.FindActionMap("Gameplay");
            blackboard.Set(InputRefs.GAMEPLAY_MAP, gameplayMap);
            
            blackboard.Set(InputRefs.VIEW, view.action);
            blackboard.Set(InputRefs.JUMP, jump.action);
            blackboard.Set(InputRefs.MOVE, move.action);
            blackboard.Set(InputRefs.SPRINT, sprint.action);
            blackboard.Set(InputRefs.SHOOT, shoot.action);
            blackboard.Set(InputRefs.CAST, cast.action);
            blackboard.Set(InputRefs.PICK_WP_01, pickWeapon01.action);
            blackboard.Set(InputRefs.PICK_WP_02, pickWeapon02.action);
        }

        void OnEnable()
        {
            asset.Enable();
            foreach (var map in asset.actionMaps) map.Enable();
        }
        void OnDisable()
        {
            asset.Disable();
            foreach (var map in asset.actionMaps) map.Disable();
        }
    }
}