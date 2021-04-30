using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome.Retro
{
    public class RetJoshAgent : RetAgent
    {
        private const string REF_COVER = "cover";

        [FoldoutGroup("Values"), SerializeField] private RetCoverProfile coverProfile;
        [FoldoutGroup("Values"), SerializeField] private float chaseTime;

        protected override void BuildTree()
        {
            var isTargetVisible = new IsVisible(REF_HEAD.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), LayerMask.GetMask("Environment"));
            var isInProximityOfTarget = new IsInProximityOf(REF_ROOT.Reference<Transform>(), REF_TARGET.Reference<Transform>(), distance);
            var hasCover = new RetHasCover(REF_ROOT.Reference<Transform>(), REF_COVER, coverProfile);
            var isCoverValid = new RetIsCoverValid(REF_COVER.Reference<RetCover>());

            var conditionalNodeOne = new CompositeConditionalNode(isTargetVisible.Chain(ConditionalOperator.AND), hasCover);
            var conditionalNodeTwo = new CompositeConditionalNode(isTargetVisible.Chain(ConditionalOperator.AND), isCoverValid);
            var conditionalNodeThree = new CompositeConditionalNode(isTargetVisible.Chain(ConditionalOperator.AND), isInProximityOfTarget);

            taskTree = new RootNode().Append(
                conditionalNodeOne.Append(
                    new RetMoveToCover(new PackettedValue<NavMeshAgent>(), REF_COVER.Reference<RetCover>(), REF_PIVOT.Reference<Transform>()).Mask(0b_0001).Append(
                        new StopMoving(new PackettedValue<NavMeshAgent>()),
                        new RootNode().Append(
                            conditionalNodeTwo.Append(
                                new RootNode().Mask(0b_0001).Append(
                                    new RetLookAt(REF_TARGETCOL.Reference<Collider>(), new PackettedValue<RetAgentLookAt>()),
                                    new ComputeDirectionTo(REF_SHOOTDIR, REF_HEAD.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), Vector3.up * 0.225f)),
                                new RetGunNode(REF_SHOOTDIR.Reference<Vector3>(), REF_FIREANCHOR.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), gun).Mask(0b_0001),
                                new RetFreeCover(REF_COVER).Mask(0b_0010)))),
                    new RetFreeCover(REF_COVER).Mask(0b_0010),
                    new RetTimedNode(chaseTime).Mask(0b_0010).Append(
                        conditionalNodeThree.Append(
                            new StopMoving(new PackettedValue<NavMeshAgent>()).Mask(0b_0001).Append(
                                new RootNode().Append(
                                    new RetLookAt(REF_TARGETCOL.Reference<Collider>(), new PackettedValue<RetAgentLookAt>()),
                                    new ComputeDirectionTo(REF_SHOOTDIR, REF_HEAD.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), Vector3.up * 0.225f)),
                                new RetGunNode(REF_SHOOTDIR.Reference<Vector3>(), REF_FIREANCHOR.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), gun)),
                            new RetMoveTo(new PackettedValue<NavMeshAgent>(), REF_TARGET.Reference<Transform>(), new PackettedValue<RetAgentLookAt>()).Mask(0b_0010).Append(
                                new Delay(reset))))));
        }
    }
}