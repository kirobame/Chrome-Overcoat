using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    [CreateAssetMenu(fileName = "NewTaskedWeapon", menuName = "Chrome Overcoat/Weapons/Tasked")]
    public class TaskedWeapon : Weapon, ITaskTree
    {
        [SerializeReference] private IWeaponBuilder builder;
        
        private ITaskTree root;
        
        public override void Build()
        {
            base.Build();
            root = builder.Build();
        }
        public override IBindable[] GetBindables() => builder.GetBindables();
        
        public override void Actualize(Packet packet) => Use(packet);
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public bool IsDone => root.IsDone;
        public bool IsLocked => root.IsLocked;
        public NodeState State => root.State;

        public int Priority
        {
            get => root.Priority;
            set => root.Priority = value;
        }

        public int Input
        {
            get => root.Input;
            set => root.Input = value;
        }
        public int Output => root.Output;

        public INode Parent
        {
            get => root.Parent;
            set => root.Parent = value;
        }
        public IReadOnlyList<INode> Children => root.Children;

        public IEnumerable<INode> UpdatedNodes => root.UpdatedNodes;
        public IEnumerable<Branch> Branches => root.Branches;

        //--------------------------------------------------------------------------------------------------------------/
        
        public override void Bootup(Packet packet)
        {
            builder.Bootup(packet);
            root.Bootup(packet);
            root.Prepare(packet);
        }

        public void Prepare(Packet packet) => root.Prepare(packet);
        public IEnumerable<INode> Use(Packet packet) => root.Use(packet);

        public void Close(Packet packet) => root.Close(packet);
        public override void Shutdown(Packet packet)
        {
            root.Shutdown(packet);
            builder.Shutdown(packet);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Insert(IEnumerable<INode> nodes) => root.Insert(nodes);

        public void Cut(IEnumerable<INode> nodes) => root.Cut(nodes);
        public void Cut(Predicate<INode> predicate) => root.Cut(predicate);

        //--------------------------------------------------------------------------------------------------------------/
        
        public bool IsUpdating(INode node) => root.IsUpdating(node);

        public void Command(Packet packet, ICommand command) => root.Command(packet, command);
        public void ActualizeCommands(Packet packet) => root.ActualizeCommands(packet);

        public void AddOutputChannel(Packet packet, int value) => root.AddOutputChannel(packet, value);
        public void RemoveOutputChannel(Packet packet, int value) => root.RemoveOutputChannel(packet, value);
        public void ChangeOutputMask(Packet packet, int value) => root.ChangeOutputMask(packet, value);
        
        //--------------------------------------------------------------------------------------------------------------/

        public int CompareTo(INode other) => root.CompareTo(other);
    }
}