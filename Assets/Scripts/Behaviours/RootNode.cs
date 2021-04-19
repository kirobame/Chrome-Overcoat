using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class RootNode : Node
    {
        public override bool IsLocked
        {
            get => isLocked || updatedNodes.Any(node => node.IsLocked);
            set => isLocked = value;
        }

        protected List<Node> updatedNodes = new List<Node>();
        protected Queue<ICommand> commands = new Queue<ICommand>();
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public override void Start(Packet packet)
        {
            IsDone = false;
            
            updatedNodes.Clear();
            foreach (var child in Childs)
            {
                if ((Output | child.Input) != Output) continue;
                
                child.Start(packet);
                updatedNodes.Add(child);
            }
            
            OnStart(packet);
        }
        protected virtual void OnStart(Packet packet) { }

        public override IEnumerable<Node> Update(Packet packet)
        {
            if (IsDone) Start(packet);
            
            OnUpdate(packet);
            UpdateCachedNodes(packet);

            if (CanBreak())
            {
                IsLocked = false;
                IsDone = true;
            }

            return null;
        }
        protected virtual void OnUpdate(Packet packet) { }
        protected void UpdateCachedNodes(Packet packet)
        {
            for (var i = 0; i < updatedNodes.Count; i++)
            {
                i = UpdateNodeAt(i, packet);
                if (i == -1) return;
            }
        }

        public override void Shutdown()
        {
            IsDone = true;
            base.Shutdown();
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Order(ICommand command) => commands.Enqueue(command);
        protected void HandleCommand(ICommand command, Packet packet)
        {
            switch (command)
            {
                case PulseCommand pulse:

                    if (updatedNodes.Contains(pulse.Target))
                    {
                        pulse.Target.Start(packet);
                        break;
                    }
                    
                    if (!pulse.Target.IsChildOf(this)) break;

                    var match = false;
                    for (var i = 0; i < updatedNodes.Count; i++)
                    {
                        if (!updatedNodes[i].IsChildOf(pulse.Target)) continue;

                        match = true;
                        updatedNodes.RemoveAt(i);
                        i--;
                    }

                    if (match)
                    {
                        pulse.Target.Start(packet);
                        updatedNodes.Add(pulse.Target);
                    }
                    break;
                
                case ShutdownCommand shutdown:
                    
                    Debug.Log("Delayed shutdown.");
                    Shutdown();
                    break;
            }
        }

        //--------------------------------------------------------------------------------------------------------------/

        protected bool CanBreak() => updatedNodes.Count == 0 || updatedNodes.All(node => node.IsDone);
        protected int UpdateNodeAt(int index, Packet packet)
        {
            var snapshot = packet.Save();
            var result = updatedNodes[index].Update(packet);
            
            if (!IsLocked)
            {
                while (commands.Count > 0)
                {
                    var command = commands.Dequeue();
                    HandleCommand(command, packet);
                }
            }

            if (IsDone) return -1;
            
            if (result == null) return index;
            updatedNodes.RemoveAt(index);

            var count = result.Count();
            if (count == 0) return index - 1;
            
            updatedNodes.InsertRange(index, result);
            for (var i = 0; i < count; i++) index = UpdateNodeAt(index + i, packet);

            packet.Load(snapshot);
            return index;
        }
    }
}