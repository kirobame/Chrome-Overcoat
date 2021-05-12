using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class CompositeConditionalNode : ConditionalNode
    {
        public CompositeConditionalNode(params ICondition[] conditions) => this.conditions = conditions;
        
        private ICondition[] conditions;

        protected override void OnBootup(Packet packet) { foreach (var condition in conditions) condition.Bootup(packet); }
        protected override void Open(Packet packet) { foreach (var condition in conditions) condition.Open(packet); }
        protected override void OnPrepare(Packet packet) { foreach (var condition in conditions) condition.Prepare(packet); }

        protected override bool Check(Packet packet)
        {
            if (!conditions.Any()) return false;
            var output = HandleCheck(packet, conditions[0]);
            
            var op = conditions[0].Operator;
            if (op == ConditionalOperator.NONE) return output;
            
            for (var i = 1; i < conditions.Length; i++)
            {
                switch (op)
                {
                    case ConditionalOperator.AND:

                        if (!output) return false;
                        output &= HandleCheck(packet, conditions[i]);
                        break;
                    
                    case ConditionalOperator.OR:

                        if (output) return true;
                        output |= HandleCheck(packet, conditions[i]);
                        break;
                }

                op = conditions[i].Operator;
                if (op == ConditionalOperator.NONE) return output;
            }

            return output;
        }

        private bool HandleCheck(Packet packet, ICondition condition) => condition.Inverse ? !condition.Check(packet) : condition.Check(packet);

        public override void OnClose(Packet packet) { foreach (var condition in conditions) condition.Close(packet); }
        protected override void OnShutdown(Packet packet) { foreach (var condition in conditions) condition.Shutdown(packet); }
    }
}