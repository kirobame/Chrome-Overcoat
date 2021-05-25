using System;
using System.Collections;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Chrome
{
    public class PauseScreen : MonoBehaviour
    {
        public bool IsPaused { get; private set; }
        
        [FoldoutGroup("Dependencies"), SerializeField] private InputHandler inputs;
        [FoldoutGroup("Dependencies"), SerializeField] private CanvasGroup group;
        [FoldoutGroup("Dependencies"), SerializeField] private Slider sensibility;

        [FoldoutGroup("Values"), SerializeField] private float fade;

        private bool state;
        
        private CachedValue<Key> key;
        private BindableGauge sensibilityBinding;

        private Coroutine routine;

        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            sensibilityBinding = Blackboard.Global.Get<BindableGauge>(ViewControl.MULTIPLIER);
            sensibility.minValue = sensibilityBinding.Range.x;
            sensibility.maxValue = sensibilityBinding.Range.y;
            sensibility.value = sensibilityBinding.Value;
            sensibility.onValueChanged.AddListener(OnSensibilityChange);
            
            key = new CachedValue<Key>(Key.Inactive);
            inputs.BindKey(InputRefs.ESCAPE, this, key);

            Events.Subscribe(GlobalEvent.OnStart, OnStart);
            Events.Subscribe(GlobalEvent.OnReset, OnReset);
            
            IsPaused = false;
        }
        void OnDestroy()
        {
            Events.Unsubscribe(GlobalEvent.OnStart, OnStart);
            Events.Unsubscribe(GlobalEvent.OnReset, OnReset);
            
            sensibility.onValueChanged.RemoveListener(OnSensibilityChange);
        }

        void Update()
        {
            if (!state || !key.IsDown()) return;
            
            var board = Blackboard.Global.Get<IBlackboard>(InputRefs.BOARD);
            var gameplayMap = board.Get<InputActionMap>(InputRefs.GAMEPLAY_MAP);
            
            if (!IsPaused)
            {
                if (routine != null) StopCoroutine(routine);
                routine = StartCoroutine(FadeRoutine(new Vector2(0.0f, 1.0f), true));

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                gameplayMap.Disable();
                Time.timeScale = 0.0f;
                IsPaused = true;
            }
            else
            {
                group.interactable = false;
                group.blocksRaycasts = false;
                
                if (routine != null) StopCoroutine(routine);
                routine = StartCoroutine(FadeRoutine(new Vector2(1.0f, 0.0f), false));
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                
                gameplayMap.Enable();
                Time.timeScale = 1.0f;
                IsPaused = false;
            }
        }

        private IEnumerator FadeRoutine(Vector2 range, bool active)
        {
            var time = 0.0f;
            if (active) time = Mathf.InverseLerp(0.0f, fade, group.alpha);
            else time = Mathf.InverseLerp(fade, 0.0f, group.alpha);
            
            while (time < fade)
            {
                time += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(range.x, range.y, time / fade);
                
                yield return new WaitForEndOfFrame();
            }

            group.interactable = active;
            group.blocksRaycasts = active;
            group.alpha = Mathf.Lerp(range.x, range.y, 1.0f);
            
            routine = null;
        }

        public void Quit() => Application.Quit();

        //--------------------------------------------------------------------------------------------------------------/

        void OnStart() => state = true;
        void OnReset() => state = false;

        void OnSensibilityChange(float value) => sensibilityBinding.Value = value;
    }
}