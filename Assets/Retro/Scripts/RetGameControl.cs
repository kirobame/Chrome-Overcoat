using System;
using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGameControl : MonoBehaviour
    {
        #region Nested Types

        [Serializable]
        private class ScreenInfo
        {
            public event Action onShowComplete;
            public event Action onHideComplete;

            public RetGameState State => state;

            [SerializeField] private RetGameState state;
            [SerializeField] private CanvasGroup group;

            public void Show()
            {
                Routines.Start(Routines.RepeatFor(0.75f, ratio =>
                {
                    group.alpha = ratio;

                }).Chain(Routines.Do(() =>
                {
                    group.alpha = 1.0f;
                    onShowComplete?.Invoke();
                    
                })));
            }

            public void Hide()
            {
                Routines.Start(Routines.RepeatFor(0.75f, ratio =>
                {
                    group.alpha = 1.0f - ratio;

                }).Chain(Routines.Do(() =>
                {
                    group.alpha = 0.0f;
                    onHideComplete?.Invoke();

                })));
            }
        }

        #endregion

        [FoldoutGroup("Values"), SerializeField] private RetGame game;
        [FoldoutGroup("Values"), SerializeField] private ScreenInfo[] infos;

        private int current;
        private bool inTransition;

        void Awake()
        {
            inTransition = true;
            current = Array.FindIndex(infos, info => info.State == RetGameState.Started);
        }

        public void SwitchTo(RetGameState state)
        {
            if (inTransition) return;
            current = Array.FindIndex(infos, info => info.State == state);

            infos[current].onShowComplete += OnShowComplete;
            infos[current].Show();

            inTransition = true;
        }
        
        public void Play()
        {
            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            var root = life.transform.root.gameObject;
            if (!root.activeInHierarchy) root.SetActive(true);
            
            game.Begin();
            infos[current].Hide();

            inTransition = false;
        }

        void OnShowComplete()
        {
            infos[current].onShowComplete -= OnShowComplete;

            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            life.End();
            
            game.Reboot();
            
        }
    }
}