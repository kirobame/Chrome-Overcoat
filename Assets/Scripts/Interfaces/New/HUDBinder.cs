using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Flux.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace Chrome
{
    public static class HUDBinder
    {
        #region Nested types

        private class HUDCandidate : IComparable<HUDCandidate>
        {
            public HUDCandidate(HUDBinding bindings, PoolableHUD prefab)
            {
                RemainingBindings = bindings ^ prefab.Value.HandledBindings;
                
                prefabs = new List<PoolableHUD>() { prefab };
                instances = new List<HUD>();
            }
            public HUDCandidate(HUDBinding bindings, HUD instance)
            {
                RemainingBindings = bindings ^ instance.HandledBindings;
                
                prefabs = new List<PoolableHUD>();
                instances = new List<HUD>() { instance };
            }

            //----------------------------------------------------------------------------------------------------------/

            public HUDBinding RemainingBindings { get; private set; }
            
            private List<PoolableHUD> prefabs;
            private List<HUD> instances;

            //----------------------------------------------------------------------------------------------------------/

            public bool TryInsert(PoolableHUD prefab)
            {
                if (!AreBindingsAcceptable(prefab.Value.HandledBindings)) return false;
                
                prefabs.Add(prefab);
                return RemainingBindings == HUDBinding.None;
            }
            public bool TryInsert(HUD instance)
            {
                if (!AreBindingsAcceptable(instance.HandledBindings)) return false;
                
                instances.Add(instance);
                return RemainingBindings == HUDBinding.None;
            }
            
            private bool AreBindingsAcceptable(HUDBinding bindings)
            {
                if ((RemainingBindings | bindings) != RemainingBindings) return false;

                RemainingBindings ^= bindings;
                return true;
            }

            //----------------------------------------------------------------------------------------------------------/

            public void Pick(RectTransform frame, IBindable[] bindables, List<HUD> destination)
            {
                var pool = Repository.Get<HUDPool>(Pool.HUD);
                foreach (var prefab in prefabs)
                {
                    var instance = pool.RequestSingle(prefab);
                    instance.BindTo(frame, bindables);
                        
                    destination.Add(instance);
                }

                foreach (var instance in instances)
                {
                    instance.UnbindFromCurrent();
                    instance.BindTo(frame, bindables);
                }
            }
            public void CleanupPreviousInstances(IEnumerable<HUD> previousInstances)
            {
                foreach (var previousInstance in previousInstances)
                {
                    if (instances.Contains(previousInstance)) continue;
                    previousInstance.Discard();
                }
            }
            
            //----------------------------------------------------------------------------------------------------------/

            public int GetScore()
            {
                var score = 0;
                foreach (var HUD in instances.Concat(prefabs.Select(prefab => prefab.Value)))
                {
                    var count = 0;
                    var bindings = HUD.HandledBindings;

                    while (bindings != 0)
                    {
                        bindings &= bindings - 1;
                        count++;
                    }

                    score += count * count;
                }

                return score;
            }
            
            public int CompareTo(HUDCandidate other) => GetScore().CompareTo(other.GetScore()) * -1;
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------/
        
        private static List<PoolableHUD> HUDPrefabs;
        private static Dictionary<HUDGroup, List<HUD>> HUDInstances;
        private static Dictionary<HUDGroup, RectTransform> HUDGroups;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootup()
        {
            HUDPrefabs = new List<PoolableHUD>();
            HUDInstances = new Dictionary<HUDGroup, List<HUD>>();
            HUDGroups = new Dictionary<HUDGroup, RectTransform>();
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static void RegisterHUDPrefab(PoolableHUD poolableHUD) => HUDPrefabs.Add(poolableHUD);
        public static void RegisterHUDGroup(HUDGroup group, RectTransform frame)
        {
            if (group == HUDGroup.None) throw new InvalidConstraintException($"On [{frame.name}] a group cannot has its value set to none!");
            if ((group & (group -1)) != 0) throw new InvalidConstraintException($"On [{frame.name}] a group cannot be assigned to multiple values!");
            if (HUDGroups.ContainsKey(group)) throw new InvalidOperationException($"On [{frame.name}] a group has already be assigned for this value!");
            
            if (group == HUDGroup.None || (group & (group -1)) != 0 || HUDGroups.ContainsKey(group)) return;
            HUDGroups.Add(group, frame);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static void Declare(HUDGroup group, params IBindable[] bindables)
        {
            if (!HUDGroups.TryGetValue(group, out var frame)) throw new InvalidOperationException($"No frame available for [{group}] group!");
            
            var bindings = HUDBinding.None;
            foreach (var bindable in bindables)
            {
                if (bindings.HasFlag(bindable.Binding)) throw new InvalidConstraintException("Same binding for multiple bindables. This is not allowed!");
                bindings |= bindable.Binding;
            }

            var successes = new List<HUDCandidate>();
            var candidates = new List<HUDCandidate>();
            var previousHUDs = new List<HUD>();
            
            if (HUDInstances.TryGetValue(group, out var existingHUDs))
            {
                previousHUDs.AddRange(existingHUDs);
                existingHUDs.Clear();

                for (var i = 0; i < previousHUDs.Count; i++)
                {
                    if ((bindings | previousHUDs[i].HandledBindings) != bindings)
                    {
                        previousHUDs[i].Discard();
                        previousHUDs.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        var candidate = new HUDCandidate(bindings, previousHUDs[i]);
                        
                        if (Process(bindings, candidates, successes, previousHUDs[i])) successes.Add(candidate);
                        else candidates.Add(candidate);
                    }
                }
            }
            else
            {
                existingHUDs = new List<HUD>();
                HUDInstances.Add(group, existingHUDs);
            }

            var query = HUDPrefabs.Where(prefab => prefab.Value.WantedGroups.HasFlag(group) && (bindings | prefab.Value.HandledBindings) == bindings);
            foreach (var prefab in query)
            {
                var candidate = new HUDCandidate(bindings, prefab);
                
                if (Process(bindings, candidates, successes, prefab.Value)) successes.Add(candidate);
                else candidates.Add(candidate);
            }
            
            if (!successes.Any()) throw new InvalidDataException("Unable to find suitable HUDs to answer to the given bindings!");

            successes.Sort();
            var chosenHUD = successes.First();
            
            chosenHUD.Pick(frame, bindables, existingHUDs);
            chosenHUD.CleanupPreviousInstances(previousHUDs);
        }

        private static bool Process(HUDBinding bindings, List<HUDCandidate> candidates, List<HUDCandidate> successes, HUD HUD)
        {
            for (var j = 0; j < candidates.Count; j++)
            {
                if (!candidates[j].TryInsert(HUD)) continue;

                successes.Add(candidates[j]);
                candidates.RemoveAt(j);
                j--;
            }
            
            return bindings == HUD.HandledBindings;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static T Get<T>(this IBindable[] bindables, HUDBinding binding) where T : IBindable
        {
            var bindable = bindables.First(candidate => candidate.Binding == binding);
            return (T)bindable;
        }
    }
}