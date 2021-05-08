using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public interface ITransform
    {
        Transform Transform { get; }
    }
    
    public interface IRoot : ITransform
    {
        event Action<IRoot> onDetachment;
        
        IRoot Parent { get; }
        
        string Tag { get; }
        Packet Packet { get; }

        IReadOnlyList<IRoot> Children { get; }

        void AttachTo(Transform parent);
        void AddChild(IRoot child);
    }

    public enum RootType : byte
    {
        Entity,
        Weapon,
        Projectile
    }

    public abstract class Root : MonoBehaviour, IRoot
    {
        public event Action<IRoot> onDetachment;
        
        public IRoot Parent { get; private set; }

        public string Tag => tag.ToString();
        [SerializeField] private RootType tag;

        public Transform Transform => transform;
        public Packet Packet { get; private set; }
        
        public IReadOnlyList<IRoot> Children => children;
        private List<IRoot> children;

        void Start()
        {
            Packet = new Packet();
            Packet.Set<IRoot>(this);

            var installers = CollectInstallers();
            Array.Sort(installers, (first, second) => first.Priority.CompareTo(second.Priority));
            foreach (var installer in installers) installer.InstallDependenciesOn(Packet);
            
            TryFetchParent();
            
            children = new List<IRoot>();
            CollectChildren();
        }
        
        public void AttachTo(Transform parent)
        {
            onDetachment?.Invoke(this);
            transform.SetParent(parent);

            TryFetchParent();
        }
        public void AddChild(IRoot child)
        {
            if (!child.Transform.IsChildOf(transform))
            {
                throw new InvalidDataException($"The root {child} isn't a child of root {this}!");
                return;
            }
            
            children.Add(child);
        }

        private void TryFetchParent()
        {
            if (transform.parent == null)
            {
                Parent = null;
                return;
            }
            
            Parent = transform.parent.GetComponentInParent<IRoot>();
            if (Parent != null) Parent.AddChild(this);
        }

        private IInstaller[] CollectInstallers()
        {
            var installers = new List<IInstaller>();
            installers.AddRange(transform.GetComponents<IInstaller>());
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                CollectInstallersFrom(child, installers);
            }

            return installers.ToArray();
        }
        private void CollectInstallersFrom(Transform transform, List<IInstaller> installers)
        {
            if (transform.HasComponent<IRoot>()) return;
            
            installers.AddRange(transform.GetComponents<IInstaller>());
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                CollectInstallersFrom(child, installers);
            }
        }
        
        private void CollectChildren()
        {
            foreach (var child in children) child.onDetachment -= OnChildDetachment;
            children.Clear();
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                CollectChildrenFrom(child);
            }
        }
        private void CollectChildrenFrom(Transform transform)
        {
            if (transform.HasComponent<IRoot>()) return;

            foreach (var injectable in transform.GetComponents<IInjectable>())
            {
                foreach (var injection in injectable.Injections) injection.FillIn(Packet);
            }

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                CollectChildrenFrom(child);
            }
        }

        void OnChildDetachment(IRoot child)
        {
            child.onDetachment -= OnChildDetachment;
            children.Remove(child);
        }
    }

    public interface IInstaller
    {
        int Priority { get; }
        
        void InstallDependenciesOn(Packet packet);
    }
    
    public interface IInjectable
    {
        IReadOnlyList<IValue> Injections { get; }
    }
}