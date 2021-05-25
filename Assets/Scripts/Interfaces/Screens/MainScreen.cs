using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Chrome
{
    public class MainScreen : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] protected string respawnPath;
        [FoldoutGroup("Dependencies"), SerializeField] protected int respawnIndex;
        
        [FoldoutGroup("Values"), SerializeField] private CanvasGroup group;
        [FoldoutGroup("Values"), SerializeField] private TMP_Text message;
        [FoldoutGroup("Values"), SerializeField] private TMP_Text button;
        [FoldoutGroup("Values"), SerializeField] private Vector2 fadeDurations;

        private Lifetime player;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake() => group.alpha = 1.0f;
        void Start()
        {
            Events.Subscribe<string>(GlobalEvent.OnReset, Appear);
            player = Repository.Get<Lifetime>(Reference.Player);
        }
        void OnDestroy() => Events.Unsubscribe<string>(GlobalEvent.OnReset, Appear);

        public void Appear(string message)
        {
            this.message.text = message;
            button.text = "Play again";

            StartCoroutine(AppearRoutine());
        }
        private IEnumerator AppearRoutine()
        {
            var time = 0.0f;
            while (time < fadeDurations.x)
            {
                time += Time.deltaTime;
                var ratio = time / fadeDurations.x;
                group.alpha = ratio;
                
                yield return new WaitForEndOfFrame();
            }
            
            group.alpha = 1.0f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        public void Disappear()
        {
            var anchors = Blackboard.Global.Get<Transform[]>(respawnPath);          
            
            player.transform.position = anchors[respawnIndex].position;
            player.transform.rotation = anchors[respawnIndex].rotation;
            player.gameObject.SetActive(true);
            player.Begin();
            
            Events.Call(GlobalEvent.OnStart);
            StartCoroutine(DisappearRoutine());
        }
        private IEnumerator DisappearRoutine()
        {
            group.interactable = false;
            group.blocksRaycasts = false;
            var time = 0.0f;
            
            while (time < fadeDurations.y)
            {
                time += Time.deltaTime;
                var ratio = 1.0f - time / fadeDurations.y;
                group.alpha = ratio;
                
                yield return new WaitForEndOfFrame();
            }
            
            group.alpha = 0.0f;
        }
        
        public void Quit() => Application.Quit();
    }
}