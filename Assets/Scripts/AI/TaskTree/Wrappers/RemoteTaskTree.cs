using System;
using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    [CreateAssetMenu(fileName = "NewRemoteTaskTree", menuName = "Chrome Overcoat/Task Tree/Remote")]
    public class RemoteTaskTree : ScriptableObject, ITaskTree, IBootable
    {
        [SerializeReference] private ITreeBuilder builder;
        
        private ITaskTree root;
        
        public void Bootup() => root = builder.Build();

        //--------------------------------------------------------------------------------------------------------------/
        
        public bool IsDone => root.IsDone;
        public bool IsLocked => root.IsLocked;
        public NodeState State => root.State;

        public int Priority => root.Priority;

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
        public IReadOnlyDictionary<int, Branch> BranchRegistry => root.BranchRegistry;

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Bootup(Packet packet) => root.Bootup(packet);

        public void Start(Packet packet) => root.Start(packet);
        public IEnumerable<INode> Update(Packet packet) => root.Update(packet);

        public void Close(Packet packet) => root.Close(packet);
        public void Shutdown(Packet packet) => root.Shutdown(packet);

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