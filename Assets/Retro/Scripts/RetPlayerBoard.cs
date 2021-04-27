using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPlayerBoard : ConcreteBoard
    {
        public const string REF_SELF = "player";
        public const string REF_BODY = "body";
        public const string REF_AIM = "aim";
        public const string REF_FIREANCHOR = "aim.fireAnchor";
        
        [FoldoutGroup("References"), SerializeField] private CharacterBody body;
        [FoldoutGroup("References"), SerializeField] private Transform aim;
        [FoldoutGroup("References"), SerializeField] private Transform fireAnchor;
        
        protected override void BuildBoard()
        {
            blackboard.Set(REF_BODY, body);
            blackboard.Set(REF_AIM, aim);
            blackboard.Set(REF_FIREANCHOR, fireAnchor);
            
            Blackboard.Global.Set<IBlackboard>(REF_SELF, this);
        }
    }
}