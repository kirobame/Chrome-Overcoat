using System.Collections;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class FinishControl : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private BoxCollider zone;
        [FoldoutGroup("Dependencies"), SerializeField] private WaveControl control;
        [FoldoutGroup("Dependencies"), SerializeField] private Transform door;

        [FoldoutGroup("Values"), SerializeField] private float height;
        [FoldoutGroup("Values"), SerializeField] private string neededWave;

        private Collider player;
        private float baseHeight;

        private bool done;
        private bool state;

        private Coroutine routine;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            baseHeight = door.transform.position.y;

            Events.Subscribe(GlobalEvent.OnStart, OnStart);
            Events.Subscribe(GlobalEvent.OnReset, OnReset);
            Events.Subscribe(AreaEvent.OnEnemyDeath, OnEnemyDeath);
        }
        void Start()
        {
            var playerBoard = Blackboard.Global.Get<IBlackboard>(PlayerRefs.BOARD);
            player = playerBoard.Get<Collider>(Refs.COLLIDER);
        }

        void Update()
        {
            if (state || !zone.bounds.Intersects(player.bounds)) return;

            Events.ZipCall(GlobalEvent.OnReset, "You won!");
            state = true;
            
            var lifetime = Repository.Get<Lifetime>(Reference.Player);
            lifetime.End();
        }

        void OnDestroy()
        {
            Events.Unsubscribe(GlobalEvent.OnStart, OnStart);
            Events.Unsubscribe(GlobalEvent.OnReset, OnReset);
            Events.Unsubscribe(AreaEvent.OnEnemyDeath, OnEnemyDeath);
        }

        //--------------------------------------------------------------------------------------------------------------/

        void OnStart()
        {
            done = false;
            state = false;
        }
        void OnReset()
        {
            if (routine != null) StopCoroutine(routine);
            
            var position = door.transform.position;
            position.y = baseHeight;

            door.transform.position = position;
        }
        
        void OnEnemyDeath()
        {
            if (done || !control[neededWave].HasBeenTriggered) return;

            routine = StartCoroutine(RaiseRoutine());
            done = true;
        }

        private IEnumerator RaiseRoutine()
        {
            var time = 0.0f;
            var goal = 1.0f;
            
            while (time < goal)
            {
                time += Time.deltaTime;

                var ratio = time / goal;
                Execute(ratio);
                
                yield return new WaitForEndOfFrame();
            }
            
            Execute(1.0f);
            routine = null;

            void Execute(float ratio)
            {
                var position = door.transform.position;
                position.y = Mathf.Lerp(baseHeight, height, ratio);
                door.transform.position = position;
            }
        }
    }
}