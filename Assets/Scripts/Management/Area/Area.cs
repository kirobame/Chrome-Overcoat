using System;
using System.Collections.Generic;
using System.Linq;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class Area : Root
    {
        public event Action onPlayerEntry;
        public event Action onPlayerExit;
        
        public int Occupancy { get; private set; }

        private bool state;
        private BoxCollider[] colliders;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void Awake()
        {
            base.Awake();
            
            Occupancy = 0;
            state = false;

            colliders = GetComponentsInChildren<BoxCollider>();
            
            if (!Repository.Exists(Reference.Areas)) Repository.Set(Reference.Areas, new List<Area>() { this });
            else Repository.AddTo(Reference.Areas, this);
        }
        protected override void HandlePacket(Packet packet) => packet.Set(this);

        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            var playerBoard = Blackboard.Global.Get<IBlackboard>(PlayerRefs.BOARD);
            var player = playerBoard.Get<Collider>(Refs.COLLIDER);

            if (!state)
            {
                if (colliders.Any(collider => collider.bounds.Intersects(player.bounds)))
                {
                    state = true;
                    
                    Events.ZipCall(AreaEvent.OnPlayerEntry, this);
                    onPlayerEntry?.Invoke();
                }
            }
            else if (colliders.All(collider => !collider.bounds.Intersects(player.bounds)))
            {
                state = false;
                
                Events.ZipCall(AreaEvent.OnPlayerExit, this);
                onPlayerExit?.Invoke();
            }
        }

        //--------------------------------------------------------------------------------------------------------------/

        public bool Contains(Collider collider) => colliders.Any(candidate => candidate.bounds.Intersects(collider.bounds));

        public void Register(Agent agent) => Occupancy++;
        public void Unregister(Agent agent)
        {
            Occupancy--;
            Events.Call(AreaEvent.OnEnemyDeath);
        }
    }
}