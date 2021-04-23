using System;
using UnityEngine;

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

        private IInteraction[] interactions;

        void Awake() => interactions = GetComponentsInChildren<IInteraction>();

        public int Relay<T>(Action<T> method) where T : IInteraction
        {
            var match = 0;
            foreach (var interaction in interactions)
            {
                if (!(interaction is T castedInteraction)) continue;

                match++;
                method(castedInteraction);
            }

            return match;
        }
    }
}