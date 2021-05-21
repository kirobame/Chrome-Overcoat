using System;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class KyleBrain : Solver
    {
        [SerializeField] private Vector2 ranges;
        [SerializeField] private Vector3 commitments;

        private IGoal idleGoal;
        private IGoal fleeGoal;
        private IGoal attackGoal;
        private IGoal seekGoal;
        
        private Transform self;
        private AreaLink selfLink;
        private Transform player;

        private bool isActive;
        private GoalDefinition activeGoal;
        private bool isActiveGoalBeingHandled;

        private float commitment;
        private float timer;

        //--------------------------------------------------------------------------------------------------------------/

        public override void Build()
        {
            idleGoal = Owner[GoalDefinition.Idle];
            fleeGoal = Owner[GoalDefinition.Flee];
            attackGoal = Owner[GoalDefinition.Attack];
            seekGoal = Owner[GoalDefinition.Seek];
            
            var playerBoard = Blackboard.Global.Get<IBlackboard>(PlayerRefs.BOARD);
            player = playerBoard.Get<Transform>(Refs.ROOT);

            var board = Owner.Identity.Packet.Get<IBlackboard>();
            self = board.Get<Transform>(Refs.ROOT);
            selfLink = Owner.Identity.Packet.Get<AreaLink>();
            
            board.Set(KyleRefs.FLEE_COOLDOWN, 0.0f);
        }

        public override void Bootup()
        {
            Events.Subscribe<Area>(AreaEvent.OnPlayerEntry, OnPlayerEntry);
            Events.Subscribe<Area>(AreaEvent.OnPlayerExit, OnPlayerExit);
            Events.Subscribe<GoalDefinition>(AgentEvent.OnGoalHandlingStart, OnGoalHandlingStart);
            
            timer = 0.0f;

            if (selfLink.Area.IsPlayerInAnyBounds) isActive = true;
            else
            {
                isActive = false;
                SwitchTo(GoalDefinition.Idle);
            }
        }
        public override void Shutdown()
        {
            Events.Unsubscribe<Area>(AreaEvent.OnPlayerEntry, OnPlayerEntry);
            Events.Unsubscribe<Area>(AreaEvent.OnPlayerExit, OnPlayerExit);
            Events.Unsubscribe<GoalDefinition>(AgentEvent.OnGoalHandlingStart, OnGoalHandlingStart);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public override void Evaluate()
        {
            if (!isActive) return;

            var board = Owner.Identity.Packet.Get<IBlackboard>();
            var isLocked = board.Get<bool>(Refs.LOCK);
            if (isLocked) return;
            
            if (activeGoal != GoalDefinition.Idle && isActiveGoalBeingHandled && timer < commitment)
            {
                timer = Mathf.Clamp(timer + Time.deltaTime, 0.0f, commitment);
                return;
            }
            
            var playerFlatPosition = new Vector2(player.position.x, player.position.z);
            var selfPosition = new Vector2(self.position.x, self.localPosition.z);
            var distance = Vector2.Distance(playerFlatPosition, selfPosition);

            var isFleeOnCooldown = IsFleeOnCooldown();
            if (distance < ranges.x)
            {
                if (activeGoal == GoalDefinition.Flee || isFleeOnCooldown) return;
                SwitchTo(GoalDefinition.Flee);
            }
            else if (distance < ranges.y)
            {
                if (activeGoal == GoalDefinition.Attack) return;
                SwitchTo(GoalDefinition.Attack);
            }
            else if (activeGoal != GoalDefinition.Seek) SwitchTo(GoalDefinition.Seek);
        }

        private void SwitchTo(GoalDefinition definition)
        {
            timer = 0.0f;
            
            activeGoal = definition;
            isActiveGoalBeingHandled = false;
            
            switch (definition)
            {
                case GoalDefinition.Idle:
                    idleGoal.IsActive = true;
                    fleeGoal.Reset();
                    attackGoal.Reset();
                    seekGoal.Reset();
                    break;

                case GoalDefinition.Flee:

                    commitment = commitments.x;
                    
                    fleeGoal.IsActive = true;
                    idleGoal.Reset();
                    attackGoal.Reset();
                    seekGoal.Reset();
                    break;
                
                case GoalDefinition.Attack:
                    
                    commitment = commitments.y;
                    
                    attackGoal.IsActive = true;
                    idleGoal.Reset();
                    fleeGoal.Reset();
                    seekGoal.Reset();
                    break;
                
                case GoalDefinition.Seek:
                    
                    commitment = commitments.z;
                    
                    seekGoal.IsActive = true;
                    idleGoal.Reset();
                    fleeGoal.Reset();
                    attackGoal.Reset();
                    break;
            }
        }

        private bool IsFleeOnCooldown()
        {
            var board = Owner.Identity.Packet.Get<IBlackboard>();
            var cooldown = board.Get<float>(KyleRefs.FLEE_COOLDOWN);

            if (cooldown > 0.0f)
            {
                cooldown -= Time.deltaTime;
                board.Set(KyleRefs.FLEE_COOLDOWN, cooldown);

                return true;
            }
            else return false;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        void OnPlayerEntry(Area area)
        {
            if (area != selfLink.Area) return;
            isActive = true;
        }
        void OnPlayerExit(Area area)
        {
            if (area != selfLink.Area) return;
            
            SwitchTo(GoalDefinition.Idle);
            isActive = false;
        }

        void OnGoalHandlingStart(GoalDefinition definition)
        {
            if (definition != activeGoal) return;
            isActiveGoalBeingHandled = true;
        }
    }
}