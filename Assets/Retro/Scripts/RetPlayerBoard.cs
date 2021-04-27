using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPlayerBoard : ConcreteBoard, ILink<IIdentity>
    {
        public IIdentity Identity => identity;
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        public const string REF_SELF = "player";
        public const string REF_BODY = "body";
        public const string REF_AIM = "aim";
        public const string REF_FIREANCHOR = "aim.fireAnchor";
        public const string REF_LIFE = "life";
        public const string REF_DETECTION = "detection";
        
        [FoldoutGroup("References"), SerializeField] private CharacterBody body;
        [FoldoutGroup("References"), SerializeField] private Transform aim;
        [FoldoutGroup("References"), SerializeField] private Transform fireAnchor;
        [FoldoutGroup("References"), SerializeField] private Lifetime life;
        [FoldoutGroup("References"), SerializeField] private RetDetectionControl detection;
        
        protected override void BuildBoard()
        {
            blackboard.Set(REF_BODY, body);
            blackboard.Set(REF_AIM, aim);
            blackboard.Set(REF_FIREANCHOR, fireAnchor);
            blackboard.Set(REF_LIFE, life);
            blackboard.Set(REF_DETECTION, detection);
            
            Blackboard.Global.Set<IBlackboard>(REF_SELF, this);
        }
    }
}