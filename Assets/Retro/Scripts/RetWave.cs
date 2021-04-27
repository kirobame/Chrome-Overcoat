using System;
using System.Collections.Generic;
using System.Linq;
using Flux.Data;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "RetWave", menuName = "Chrome Overcoat/Retro/Wave")]
    public class RetWave : ScriptableObject
    {
        #region Nested Types

        [Serializable]
        private class SpawnInfo
        {
            public int Count => count;
            public RetWaveSpawnAddress Address => address;
            
            [SerializeField] private PoolableEnemy enemyPrefab;
            [SerializeField] private int count;
            [SerializeField] private RetWaveSpawnAddress address;

            public int Accepts(RetWaveSpawnAnchor anchor)
            {
                var filter = anchor.Address & address;
                if (filter != 0 && !Physics.CheckSphere(anchor.transform.position, 1.5f, LayerMask.GetMask("Entity")))
                {
                    var cost = filter ^ anchor.Address;
                    return cost != 0 ? 1 : 0;
                }
                else return -1;
            }
            public PoolableEnemy Assign(RetWaveSpawnAnchor anchor)
            {
                var enemyPool = Repository.Get<EnemyPool>(RetReference.EnemyPool);
                var enemyPoolable = enemyPool.RequestSinglePoolable(enemyPrefab);
                
                enemyPoolable.transform.position = anchor.transform.position;
                return enemyPoolable;
            }
        }
        #endregion

        public event Action<RetWave> onPartiallyComplete;
        public event Action<RetWave> onComplete;
        
        [SerializeField] private SpawnInfo[] infos;
        [SerializeField] private int completion;

        private int total;
        private int progress;
        
        private bool hasBeenPartiallyCompleted;
        
        public void Execute()
        {
            Debug.Log($"Starting {this}");

            progress = 0;
            total = 0;
            
            hasBeenPartiallyCompleted = false;

            var anchors = Repository.GetAll<RetWaveSpawnAnchor>(RetReference.WaveSpawns).ToList();
            var costs = new List<int>();
            var availableAnchors = new Dictionary<int, List<RetWaveSpawnAnchor>>();
            
            foreach (var info in infos)
            {
                costs.Clear();
                availableAnchors.Clear();
                
                foreach (var anchor in anchors)
                {
                    var score = info.Accepts(anchor);
                    if (score == -1) continue;
                    
                    if (availableAnchors.TryGetValue(score, out var list)) list.Add(anchor);
                    else
                    {
                        availableAnchors.Add(score, new List<RetWaveSpawnAnchor>() {anchor});
                        costs.Add(score);
                    }
                }
                
                costs.Sort();
                var count = info.Count;
                
                while (costs.Count > 0 && count > 0)
                {
                    while (availableAnchors[costs[0]].Count <= 0) costs.RemoveAt(0);

                    var list = availableAnchors[costs[0]];
                    var index = Random.Range(0, list.Count);
                    
                    var enemy = info.Assign(list[index]);
                    enemy.onDeath += OnEnemyDeath;
                    
                    anchors.Remove(list[index]);
                    list.RemoveAt(index);

                    count--;
                    total++;
                }
            }
        }

        void OnEnemyDeath(PoolableEnemy enemy)
        {
            Debug.Log($"{enemy} died -> {progress + 1} / {completion}");
            
            enemy.onDeath -= OnEnemyDeath;
            progress++;
            
            if (progress >= completion && !hasBeenPartiallyCompleted)
            {
                onPartiallyComplete?.Invoke(this);
                hasBeenPartiallyCompleted = true;
            }
            if (progress >= total) onComplete?.Invoke(this);
        }
    }
}