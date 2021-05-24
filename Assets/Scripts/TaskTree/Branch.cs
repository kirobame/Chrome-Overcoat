using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Chrome
{
    public class Branch
    {
        private const int STOP_CODE = -42;
        
        public Branch(ITaskTree source, int key)
        {
            Key = key;
            this.source = source;
        }
        public Branch(ITaskTree source, int key, params INode[] nodes)
        {
            Key = key;
            
            actives.AddRange(nodes);
            actives.Sort(CompareNodes);
            
            this.source = source;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public bool IsDone => actives.Count == 0 || actives.All(node => node is RootNode && node.IsDone);

        public int Key { get; private set; }
        public IEnumerable<INode> All => actives.Concat(leaves);

        public IReadOnlyList<INode> Nodes => actives;
        private List<INode> actives = new List<INode>();

        public IReadOnlyList<INode> Leaves => leaves;
        private List<INode> leaves = new List<INode>();
        
        private ITaskTree source;

        //--------------------------------------------------------------------------------------------------------------/

        public void Update(Packet packet)
        {
            for (var i = 0; i < actives.Count; i++)
            {
                i = UpdateAt(packet, i);
                if (i == STOP_CODE) return;
            }
        }
        
        private int UpdateAt(Packet packet, int index)
        {
            var current = actives[index];
            var results = current.Use(packet);
            
            source.ActualizeCommands(packet);
            if (actives.Count == 0) return STOP_CODE;

            if (results == null) return index;
            actives.RemoveAt(index);

            if (!results.Any())
            {
                leaves.Add(current);
                return index - 1;
            }

            var sortedResults = results.ToList();
            sortedResults.Sort(CompareNodes);
            actives.InsertRange(index, sortedResults);
;
            return -1;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void Add(INode node)
        {
            actives.Add(node);
            actives.Sort(CompareNodes);
        }

        public void Remove(INode node) => actives.Remove(node);
        public void RemoveAt(int index) => actives.RemoveAt(index);

        public void Reset()
        {
            actives.Clear();
            leaves.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------/

        private int CompareNodes(INode first, INode second) => first.Priority.CompareTo(second.Priority);
    }
}