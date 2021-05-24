using System;
using System.Collections.Generic;
using System.Linq;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class WaveControl : MonoBehaviour, IInstaller, IInjectable, IInjectionCallbackListener
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            spawners = new AnyValue<Dictionary<SpawnLocations, Spawner>>();
            area = new AnyValue<Area>();
            
            injections = new IValue[]
            {
                spawners,
                area
            };
        }
        
        void IInjectionCallbackListener.OnInjectionDone(IRoot source)
        {
            foreach (var wave in waves) wave.AssignTo(this);
            
            area.Value.onPlayerEntry += OnPlayerEntry;
            area.Value.onPlayerExit += OnPlayerExit;
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        #region Nested Types

        [Serializable]
        public class Wave : IAssignable<WaveControl>
        {
            public object Value => Owner;
            public WaveControl Owner { get; private set; }
            
            public bool HasBeenTriggered { get; private set; }
            
            public string Name => name;
            [SerializeField] private string name;
            
            [SerializeReference] private ICondition[] conditions = new ICondition[0];
            [SerializeField] private RefreshToken[] refreshes;
            [SerializeField] private Spawn[] spawns;

            private Packet packet => Owner.Packet;

            //--------------------------------------------------------------------------------------------------------------/

            public void AssignTo(WaveControl owner)
            {
                HasBeenTriggered = false;
                
                Owner = owner;
                foreach (var spawn in spawns) spawn.AssignTo(this);
            }

            public void Bootup()
            {
                foreach (var refresh in refreshes)
                {
                    refresh.Bootup();
                    refresh.onActivation += TryExecute;
                }
                foreach (var condition in conditions)
                {
                    condition.Bootup(packet);
                    condition.Open(packet);
                    condition.Prepare(packet);
                }
            }
            public void Shutdown()
            {
                foreach (var refresh in refreshes)
                {
                    refresh.onActivation -= TryExecute;
                    refresh.Shutdown();
                }
                foreach (var condition in conditions)
                {
                    condition.Close(packet);
                    condition.Shutdown(packet);
                }
            }
            
            private void TryExecute()
            {
                if (conditions.Any(condition => !condition.Check(packet))) return;
                
                //Debug.Log($"Spawning [{name}] wave in [{Owner.Area.Transform.gameObject.name}] area");
                foreach (var spawn in spawns) spawn.Execute();
                HasBeenTriggered = true;
                
                Shutdown();
            }
        }

        [Serializable]
        private class Spawn : IAssignable<Wave>
        {
            public object Value => Owner;
            public Wave Owner { get; private set; }
            
            [SerializeField] private SpawnLocations locations;
            [SerializeField] private PoolableAgent agentPrefab;
            [SerializeReference] private IAgentModification[] modifications = new IAgentModification[0];

            //--------------------------------------------------------------------------------------------------------------/

            public void AssignTo(Wave owner) => Owner = owner;
            
            public void Execute()
            {
                var control = Owner.Recurse<WaveControl>();
                var agentPool = Repository.Get<AgentPool>(Pool.Agent);
                
                foreach (var location in locations.Split())
                {
                    if (!control.TryGetSpawner(location, out var spawner)) continue;

                    var agentInstance = agentPool.RequestSingle(agentPrefab);
                    var link = agentInstance.Identity.Packet.Get<AreaLink>();
                    link.Set(control.Area);

                    foreach (var modification in modifications) modification.Modify(agentInstance);
                    spawner.Spawn(agentInstance);
                }
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------/

        public Area Area => area.Value;
        public Packet Packet => Area.Packet;

        [SerializeField] private bool isActive = true;
        [SerializeField] private Wave[] waves;
        
        private IValue<Dictionary<SpawnLocations, Spawner>> spawners;
        private IValue<Area> area;

        public Wave this[string name] => waves.First(wave => wave.Name == name);

        //--------------------------------------------------------------------------------------------------------------/
        
        void OnDestroy()
        {
            area.Value.onPlayerEntry -= OnPlayerEntry;
            area.Value.onPlayerExit -= OnPlayerExit;
        }

        //--------------------------------------------------------------------------------------------------------------/

        void OnPlayerEntry()
        {
            if (!isActive) return;
            
            foreach (var wave in waves)
            {
                if (wave.HasBeenTriggered) continue;
                wave.Bootup();
            }
        }
        void OnPlayerExit()
        {
            if (!isActive) return;
            
            foreach (var wave in waves)
            {
                if (wave.HasBeenTriggered) continue;
                wave.Shutdown();
            }
        }

        //--------------------------------------------------------------------------------------------------------------/

        public bool TryGetSpawner(SpawnLocations location, out Spawner spawner) => spawners.Value.TryGetValue(location, out spawner);

        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}