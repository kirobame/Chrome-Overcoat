using System;
using UnityEngine;

namespace Chrome
{
    public class GaugeInRangeModule : GaugeModule
    {
        #region Nested Types

        public enum State
        {
            EnteredRange,
            InRange,
            LeftRange
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------/

        public GaugeInRangeModule(Vector2 range, Action<float, float, State> method, bool inPercent = true)
        {
            this.range = range;
            this.inPercent = inPercent;

            this.method = method;
        }
        
        private Vector2 range;
        private bool inPercent;

        private Action<float, float, State> method;
        private State state = State.LeftRange;
        
        //--------------------------------------------------------------------------------------------------------------/

        public override void Update()
        {
            base.Update();

            bool check;
            float percentage;
            
            if (inPercent)
            {
                var min = range.x * Source.Max;
                var max = range.y * Source.Max;

                percentage = Mathf.InverseLerp(min, max, Source.Value);
                check = Source.Value < min || Source.Value > max;
            }
            else
            {
                percentage = Mathf.InverseLerp(range.x, range.y, Source.Value);
                check = Source.Value < range.x || Source.Value > range.y;
            }
            
            if (check)
            {
                if (state == State.InRange)
                {
                    method(Source.Value, percentage, State.LeftRange);
                    state = State.LeftRange;
                }
                    
                return;
            }

            if (state == State.LeftRange) state = State.EnteredRange;
            else state = State.InRange;

            method(Source.Value, percentage, state);
        }
    }
}