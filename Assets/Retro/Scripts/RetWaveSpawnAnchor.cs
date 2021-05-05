using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetWaveSpawnAnchor : MonoBehaviour
    {
        public RetWaveSpawnAddress Address => address;
        [FoldoutGroup("Values"), SerializeField] private RetWaveSpawnAddress address;

        void Awake()
        {
            if (!Repository.Exists(RetReference.WaveSpawns)) Repository.Set(RetReference.WaveSpawns, new List<RetWaveSpawnAnchor>());
            Repository.AddTo(RetReference.WaveSpawns, this);
        }
    }
}