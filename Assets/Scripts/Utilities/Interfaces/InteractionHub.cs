using System;
using UnityEngine;
using System.Collections.Generic;

namespace Chrome
{
    public class InteractionHub : MonoBehaviour, ILink<IIdentity>
    {
        public IIdentity Identity => identity;
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        //private IInteraction[] interactions;
        //void Awake() => interactions = GetComponentsInChildren<IInteraction>();

        private List<IInteraction> interactionList = new List<IInteraction>();
        void Awake() => GetIInteractions(this.transform);

        void GetIInteractions(Transform tr)
        {
            if (tr.GetComponent<InteractionHub>() != null && tr.GetComponent<InteractionHub>() != this) return;

            IInteraction[] interactions = tr.GetComponents<IInteraction>();
            if (interactions.Length > 0)
                foreach (var interaction in interactions)
                    if (interaction != null && !interactionList.Contains(interaction))
                        interactionList.Add(interaction);

            if (tr.childCount > 0)
                foreach (Transform child in tr)
                    GetIInteractions(child);
        }

        public int Relay<T>(Action<T> method) where T : IInteraction
        {
            var match = 0;
            foreach (var interaction in interactionList)
            {
                if (!(interaction is T castedInteraction)) continue;

                match++;
                method(castedInteraction);
            }

            return match;
        }
    }
}