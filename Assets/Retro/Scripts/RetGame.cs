using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "RetGame", menuName = "Chrome Overcoat/Retro/Game")]
    public class RetGame : ScriptableObject
    {
        [SerializeField] private RetWave[] waves;

        private int progress;
        
        public void Begin()
        {
            progress = 0;

            waves[0].onPartiallyComplete += OnWavePartiallyComplete;
            waves[0].Execute();
        }

        void OnWavePartiallyComplete(RetWave wave)
        {
            wave.onPartiallyComplete -= OnWavePartiallyComplete;

            if (progress + 1 >= waves.Length)
            {
                wave.onComplete += OnWaveComplete;
                return;
            }
            
            progress++;
            waves[progress].onPartiallyComplete += OnWavePartiallyComplete;
            waves[progress].Execute();
        }

        void OnWaveComplete(RetWave wave)
        {
            wave.onComplete -= OnWaveComplete;
            Debug.Log("Win !");
        }
    }
}