using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPlayerBoard : ConcreteBoard
    {
        public const string REF_SELF = "player";
        public const string REF_BODY = "body";
        
        [FoldoutGroup("References"), SerializeField] private CharacterBody body;
        
        protected override void BuildBoard()
        {
            blackboard.Set(REF_BODY, body);
            Blackboard.Global.Set<IBlackboard>(REF_SELF, this);
        }
    }
}