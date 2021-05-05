using System;
using Flux.Event;
using Flux.Feedbacks;
using UnityEngine;

namespace Chrome
{
    public class Lifetime : MonoBehaviour, ILifebound
    {
        [SerializeField] private Sequence sequence;
        public bool bypassBootup;
        
        private SendbackArgs args;
        private bool hasBeenBootedUp;
        
        //private ILifebound[] lifebounds;
        private List<ILifebound> lifeboundsList = new List<ILifebound>();
        private List<Lifetime> subLifetimesList = new List<Lifetime>();

        private List<ILifebound> lifebounds;
        void Awake()
        {
            hasBeenBootedUp = false;
            
            args = new SendbackArgs();
            args.onDone += OnSequenceDone;

            GetILifebounds(this.transform);
        }

        void GetILifebounds(Transform tr)
        {
            var otherLifetime = tr.GetComponent<Lifetime>();
            if (otherLifetime != null && otherLifetime != this)
            {
                subLifetimesList.Add(otherLifetime);
                return;
            }

            ILifebound[] lifebounds = tr.GetComponents<ILifebound>();
            if (lifebounds.Length > 0)
                foreach (var lifebound in lifebounds)
                    if (lifebound != null && lifebound != this && !lifeboundsList.Contains(lifebound))
                        lifeboundsList.Add(lifebound);

            if (tr.childCount > 0)
                foreach (Transform child in tr)
                    GetILifebounds(child);
        }

        void OnEnable()
        {
            if (!hasBeenBootedUp && !bypassBootup)
            {
                hasBeenBootedUp = true;
                return;
            }
            foreach (var lifebound in lifeboundsList) lifebound.Bootup();
            foreach (var subLifetime in subLifetimesList)
            {
                subLifetime.gameObject.SetActive(true);
                subLifetime.Bootup();
            }
        }
        
        public void End()
        {
            foreach (var lifebound in lifeboundsList) lifebound.Shutdown();
            sequence.Play(args);
        }

        void OnSequenceDone(EventArgs args) => gameObject.SetActive(false);

        public void Bootup()
        {
        }

        public void Shutdown()
        {
        }
    }
}