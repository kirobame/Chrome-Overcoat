using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chrome
{
    public class InteractionHub : MonoBehaviour, IInstaller, IInjectable, IInjectionCallbackListener
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
        void IInjectionCallbackListener.OnInjectionDone(IRoot source)
        {
            root = source;
            source.onAttachment += OnRootAttachment;
            source.onDetachment += OnRootDetachment;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private IRoot root;
        
        public IIdentity Identity => identity.Value;
        private IValue<IIdentity> identity;
        
        private List<IInteraction> interactions;
        
        private InteractionHub parent;
        private List<InteractionHub> children;

        //--------------------------------------------------------------------------------------------------------------/
        
        void Awake()
        {
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };
            
            children = new List<InteractionHub>();
            
            interactions = transform.Fetch<IInteraction,InteractionHub>().ToList();
            foreach (var interaction in interactions) interaction.onDestruction += RemoveInteraction;
        }
        void Start() => TryFetchParent();
        
        void OnDestroy()
        {
            root.onAttachment -= OnRootAttachment;
            root.onDetachment -= OnRootDetachment;
            
            foreach (var interaction in interactions) interaction.onDestruction -= RemoveInteraction;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void AddChild(InteractionHub child)
        {
            if (!child.transform.IsChildOf(transform))
            {
                throw new InvalidDataException($"The InteractionHub : {child} isn't a child of InteractionHub : {this}!");
                return;
            }

            children.Add(child);
        }
        public void RemoveChild(InteractionHub child) => children.Remove(child);
        
        private void TryFetchParent()
        {
            if (transform.parent == null)
            {
                parent = null;
                return;
            }
            
            parent = transform.parent.GetComponentInParent<InteractionHub>();
            if (parent != null) parent.AddChild(this);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public void AddInteraction(IInteraction interaction)
        {
            interaction.onDestruction += RemoveInteraction;
            interactions.Add(interaction);
        }
        public void RemoveInteraction(IInteraction interaction)
        {
            interactions.Remove(interaction);
            interaction.onDestruction -= RemoveInteraction;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public int Relay<T>(Func<T,int,bool> method, int depth = 0) where T : IInteraction
        {
            var match = 0;
            foreach (var interaction in interactions)
            {
                if (!interaction.IsActive || !(interaction is T castedInteraction)) continue;
                if (method(castedInteraction, depth)) match++;
            }

            foreach (var child in children) match += child.Relay<T>(method, depth + 1);
            return match;
        }

        //--------------------------------------------------------------------------------------------------------------/

        void OnRootAttachment(IRoot root) => TryFetchParent();
        void OnRootDetachment(IRoot root)
        {
            if (parent != null) parent.RemoveChild(this);
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 0;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set<InteractionHub>(this);
    }
}