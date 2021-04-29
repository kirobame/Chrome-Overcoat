using System;
using Flux;
using Flux.Data;
using Flux.Event;
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
                    group.interactable = true;
                    group.blocksRaycasts = true;
                    
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
                    group.interactable = false;
                    group.blocksRaycasts = false;
                    
                    onHideComplete?.Invoke();

                })));
            }
        }

        #endregion

        [FoldoutGroup("Values"), SerializeField] private RetGame game;
        [FoldoutGroup("Values"), SerializeField] private ScreenInfo[] infos;

        private int current;
        private bool inTransition;
        private bool inShow;

        void Awake()
        {
            inTransition = true;
            inShow = true;
            
            current = Array.FindIndex(infos, info => info.State == RetGameState.Started);
        }

        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Z) || !inShow) return;
            Play();
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
            inShow = false;
        }

        void OnShowComplete()
        {
            inShow = true;
            infos[current].onShowComplete -= OnShowComplete;

            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            life.End();
            
            Events.Call(RetEvent.OnGameEnd);
            game.Reboot();
        }
    }
}