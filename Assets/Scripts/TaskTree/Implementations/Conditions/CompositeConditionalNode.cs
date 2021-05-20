using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CompositeConditionalNode : ConditionalNode
    {
        #region Nested types

        [Serializable]
        public class ConditionOperatorPair
        {
            public ConditionOperatorPair(ICondition condition)
            {
                this.condition = condition;
                this.conditionalOperator = ConditionalOperator.NONE;
            }
            public ConditionOperatorPair(ICondition condition, ConditionalOperator conditionalOperator)
            {
                this.condition = condition;
                this.conditionalOperator = conditionalOperator;
            }

            public ConditionalOperator Operator
            {
                get => conditionalOperator;
                set => conditionalOperator = value;
            }
            public bool Inverse => inverse;
            
            [SerializeReference] private ICondition condition;
            [SerializeField] private bool inverse;
            [LabelText("Operator"), SerializeField] private ConditionalOperator conditionalOperator;

            public void Bootup(Packet packet) => condition.Bootup(packet);
            public void Open(Packet packet) => condition.Open(packet);
            
            public void Prepare(Packet packet) => condition.Prepare(packet);
            public bool Check(Packet packet) => condition.Check(packet);
            
            public void Close(Packet packet) => condition.Close(packet);
            public void Shutdown(Packet packet) => condition.Shutdown(packet);
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------/
        
        public CompositeConditionalNode(params (ICondition condition, ConditionalOperator conditionalOperator)[] tuples)
        {
           conditions = new List<ConditionOperatorPair>(tuples.Length);
            foreach (var tuple in tuples) this.conditions.Add(new ConditionOperatorPair(tuple.condition, tuple.conditionalOperator));
        }

        [SerializeField] private List<ConditionOperatorPair> conditions;
        
        //--------------------------------------------------------------------------------------------------------------/

        public void Add(ConditionalOperator conditionalOperator, ICondition condition)
        {
            if (conditions.Any()) conditions[conditions.Count - 1].Operator = conditionalOperator;
            conditions.Add(new ConditionOperatorPair(condition));
        } 
        
        protected override void OnBootup(Packet packet) { foreach (var condition in conditions) condition.Bootup(packet); }
        protected override void Open(Packet packet) { foreach (var condition in conditions) condition.Open(packet); }
        protected override void OnPrepare(Packet packet) { foreach (var condition in conditions) condition.Prepare(packet); }

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override bool Check(Packet packet)
        {
            if (!conditions.Any()) return false;
            var output = HandleCheck(packet, conditions[0]);
            
            var op = conditions[0].Operator;
            if (op == ConditionalOperator.NONE) return output;
            
            for (var i = 1; i < conditions.Count; i++)
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

        private bool HandleCheck(Packet packet, ConditionOperatorPair condition) => condition.Inverse ? !condition.Check(packet) : condition.Check(packet);

        //--------------------------------------------------------------------------------------------------------------/
        
        public override void OnClose(Packet packet) { foreach (var condition in conditions) condition.Close(packet); }
        protected override void OnShutdown(Packet packet) { foreach (var condition in conditions) condition.Shutdown(packet); }
    }
}