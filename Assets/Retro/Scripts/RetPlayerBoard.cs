using Flux;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPlayerBoard : RuntimeBoard
    {
        public IIdentity Identity => identity;

        public const string REF_SELF = "player";
        public const string REF_BODY = "body";
        public const string REF_COLLIDER = "collider";
        public const string REF_AIM = "aim";
        public const string REF_FIREANCHOR = "aim.fireAnchor";
        public const string REF_LIFE = "life";
        public const string REF_DETECTION = "detection";
        public const string REF_IDENTITY = "identity";

        [FoldoutGroup("References"), SerializeField] private Identity identity;
        [FoldoutGroup("References"), SerializeField] private CharacterBody body;
        [FoldoutGroup("References"), SerializeField] private Transform aim;
        [FoldoutGroup("References"), SerializeField] private Lifetime life;
        [FoldoutGroup("References"), SerializeField] private RetDetectionControl detection;
        [FoldoutGroup("References"), SerializeField] private new Collider collider;
        
        /*protected override void BuildBoard()
        {
            blackboard.Set(REF_BODY, body);
            blackboard.Set(REF_AIM, aim);
            blackboard.Set(REF_LIFE, life);
            blackboard.Set(REF_DETECTION, detection);
            blackboard.Set(REF_IDENTITY, identity);
            blackboard.Set(REF_COLLIDER, collider);
            
            Blackboard.Global.Set<IBlackboard>(REF_SELF, this);
        }*/
    }
}