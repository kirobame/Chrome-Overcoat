using System;
using Flux;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

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

        public static bool isLocked;
        
        [FoldoutGroup("Values"), SerializeField] private RetGame game;
        [FoldoutGroup("Values"), SerializeField] private ScreenInfo[] infos;

        [FoldoutGroup("Feedbacks"), SerializeField] private AudioMixerSnapshot menuSnapshot;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioMixerSnapshot inGameSnapshot;
        [FoldoutGroup("Feedbacks"), SerializeField] private Vector2 transitions;

        private int current;
        private bool inTransition;
        private bool inShow;
        private bool inHide;

        void Awake()
        {
            isLocked = false;
            
            inTransition = true;
            inShow = true;
            inHide = false;
            
            current = Array.FindIndex(infos, info => info.State == RetGameState.Started);
        }

        void Update()
        {
            if (!inShow) return;
            
            if (Input.GetKeyDown(KeyCode.Z) && !isLocked && !inHide) Play();
            if (Input.GetKeyDown(KeyCode.Escape)) Quit();
        }
        
        public void SwitchTo(RetGameState state)
        {
            if (inTransition) return;
            
            if (state == RetGameState.Won) Events.Call(RetEvent.OnGameWon);
            else if (state == RetGameState.Lost) Events.Call(RetEvent.OnGameLost);
            current = Array.FindIndex(infos, info => info.State == state);

            infos[current].onShowComplete += OnShowComplete;
            infos[current].Show();

            menuSnapshot.TransitionTo(transitions.x);
            inTransition = true;
        }
        
        public void Play()
        {
            inHide = true;
            
            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            var root = life.transform.root.gameObject;
            if (!root.activeInHierarchy) root.SetActive(true);
            
            Routines.Start(Routines.DoAfter(() => game.Begin(), game.StartDelay));
            Events.Call(RetEvent.OnGameStart);
            
            infos[current].Hide();
            
            inGameSnapshot.TransitionTo(transitions.y);
            inTransition = false;
            inShow = false;
        }

        public void Quit()
        {
            Debug.Log("QUIT !");
            Application.Quit();
        }

        void OnShowComplete()
        {
            infos[current].onShowComplete -= OnShowComplete;
            
            inHide = false;
            inShow = true;

            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
            life.End();
            
            Events.Call(RetEvent.OnGameEnd);
            Events.Call(RetEvent.OnScreenDisplay);
            
            game.Reboot();
        }
    }
}