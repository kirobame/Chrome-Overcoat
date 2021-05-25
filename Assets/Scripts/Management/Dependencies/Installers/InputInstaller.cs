using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class InputInstaller : MonoBehaviour
    {
        [FoldoutGroup("Values"), SerializeField] private InputActionAsset asset;
        
        [FoldoutGroup("General"), SerializeField] private InputActionReference escape;

        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference view;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference jump;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference move;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference sprint;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference shoot;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference pickup;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference stomp;
        
        private Blackboard blackboard;
        
        void Awake()
        {
            blackboard = new Blackboard();
            Blackboard.Global.Set<IBlackboard>(InputRefs.BOARD, blackboard);

            var generalMap = asset.FindActionMap("General");
            generalMap.Enable();
            blackboard.Set(InputRefs.GENERAL_MAP, generalMap);
            
            blackboard.Set(InputRefs.ESCAPE, escape.action);
            
            var gameplayMap = asset.FindActionMap("Gameplay");
            gameplayMap.Enable();
            blackboard.Set(InputRefs.GAMEPLAY_MAP, gameplayMap);
            
            blackboard.Set(InputRefs.VIEW, view.action);
            blackboard.Set(InputRefs.JUMP, jump.action);
            blackboard.Set(InputRefs.MOVE, move.action);
            blackboard.Set(InputRefs.SPRINT, sprint.action);
            blackboard.Set(InputRefs.SHOOT, shoot.action);
            blackboard.Set(InputRefs.PICKUP, pickup.action);
            blackboard.Set(InputRefs.STOMP, stomp.action);
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