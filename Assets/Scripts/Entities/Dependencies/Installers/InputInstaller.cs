using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class InputInstaller : MonoBehaviour
    {
        [FoldoutGroup("Values"), SerializeField] private InputActionAsset asset;
        
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference jump;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference move;
        [FoldoutGroup("Gameplay"), SerializeField] private InputActionReference shoot;

        private Blackboard blackboard;
        
        void Awake()
        {
            blackboard = new Blackboard();
            Blackboard.Global.Set<IBlackboard>(InputRefs.BOARD, blackboard);

            var gameplayMap = asset.FindActionMap("Gameplay");
            blackboard.Set(InputRefs.GAMEPLAY_MAP, gameplayMap);
            
            blackboard.Set(InputRefs.JUMP, jump.action);
            blackboard.Set(InputRefs.MOVE, move.action);
            blackboard.Set(InputRefs.SHOOT, shoot.action);
        }

        void OnEnable() => asset.Enable();
        void OnDisable() => asset.Disable();
    }
}