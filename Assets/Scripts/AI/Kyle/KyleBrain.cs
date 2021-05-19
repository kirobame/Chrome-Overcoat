using System;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class KyleBrain : Solver
    {
        [SerializeField] private Vector3 ranges;
        [SerializeField] private float commitment;

        private IGoal idleGoal;
        private IGoal fleeGoal;
        private IGoal attackGoal;
        private IGoal seekGoal;
        
        private Transform self;
        private AreaLink selfLink;
        
        private Transform player;

        private bool isActive;
        private GoalDefinition activeGoal;
        private float timer;

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
        }

        public override void Bootup()
        {
            Events.Subscribe<Area>(AreaEvent.OnPlayerEntry, OnPlayerEntry);
            Events.Subscribe<Area>(AreaEvent.OnPlayerExit, OnPlayerExit);
            
            isActive = false;
            timer = 0.0f;
            
            SwitchTo(GoalDefinition.Idle);
        }
        public override void Shutdown()
        {
            Events.Unsubscribe<Area>(AreaEvent.OnPlayerEntry, OnPlayerEntry);
            Events.Unsubscribe<Area>(AreaEvent.OnPlayerExit, OnPlayerExit);
        }

        public override void Evaluate()
        {
            if (!isActive) return;
            
            if (activeGoal != GoalDefinition.Idle && timer < commitment)
            {
                timer += Mathf.Clamp(timer + Time.deltaTime, 0.0f, commitment);
                return;
            }
            
            var playerFlatPosition = new Vector2(player.position.x, player.position.z);
            var selfPosition = new Vector2(self.position.x, self.localPosition.z);

            var distance = Vector2.Distance(playerFlatPosition, selfPosition);
            
            if (distance < ranges.x) SwitchTo(GoalDefinition.Flee);
            else if (distance < ranges.y) SwitchTo(GoalDefinition.Attack);
            else SwitchTo(GoalDefinition.Seek);
        }

        private void SwitchTo(GoalDefinition definition)
        {
            timer = 0.0f;
            activeGoal = definition;
            
            switch (definition)
            {
                case GoalDefinition.Idle:
                    idleGoal.IsActive = true;
                    fleeGoal.Reset();
                    attackGoal.Reset();
                    seekGoal.Reset();
                    
                    break;

                case GoalDefinition.Flee:
                    fleeGoal.IsActive = true;
                    idleGoal.Reset();
                    attackGoal.Reset();
                    seekGoal.Reset();
                    
                    break;
                
                case GoalDefinition.Attack:
                    attackGoal.IsActive = true;
                    idleGoal.Reset();
                    fleeGoal.Reset();
                    seekGoal.Reset();
                    
                    break;
                
                case GoalDefinition.Seek:
                    seekGoal.IsActive = true;
                    idleGoal.Reset();
                    fleeGoal.Reset();
                    attackGoal.Reset();

                    break;
            }
        }

        void OnPlayerEntry(Area area)
        {
            if (area != selfLink.Value) return;
            isActive = true;
        }
        void OnPlayerExit(Area area)
        {
            if (area != selfLink.Value) return;
            
            SwitchTo(GoalDefinition.Idle);
            isActive = false;
        }
    }
}