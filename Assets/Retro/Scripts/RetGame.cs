using System.Linq;
using Flux.Audio;
using Flux.Data;
using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "RetGame", menuName = "Chrome Overcoat/Retro/Game")]
    public class RetGame : ScriptableObject
    {
        public float StartDelay => startDelay;

        [SerializeField] private AudioPackage sound;
        [SerializeField] private float startDelay;
        [SerializeField] private RetWave[] waves;

        private int progress;
        
        public void Begin()
        {
            progress = 0;

            Debug.Log($"Playing first wave");
            waves[0].onPartiallyComplete += OnWavePartiallyComplete;
            if (waves[0].Execute()) sound.Play();
        }

        public void Reboot()
        {
            waves[progress].onPartiallyComplete -= OnWavePartiallyComplete;
            waves[progress].onComplete -= OnWaveComplete;
            
            foreach (var wave in waves) wave.Reboot();
        }

        void OnWavePartiallyComplete(RetWave wave)
        {
            wave.onPartiallyComplete -= OnWavePartiallyComplete;

            if (progress + 1 >= waves.Length)
            {
                Debug.Log($"Waiting for last wave end");
                wave.onComplete += OnWaveComplete;
                return;
            }
            
            progress++;
            Debug.Log($"Playing wave : {waves[progress]}");
            waves[progress].onPartiallyComplete += OnWavePartiallyComplete;
            if (waves[progress].Execute()) sound.Play();
        }

        void OnWaveComplete(RetWave wave)
        {
            wave.onComplete -= OnWaveComplete;

            var game = Repository.Get<RetGameControl>(RetReference.Game);
            game.SwitchTo(RetGameState.Won);
        }
    }
}