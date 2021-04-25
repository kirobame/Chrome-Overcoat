using UnityEngine;

namespace Chrome
{
    public class ScaleBetween : LerpNode
    {
        public ScaleBetween(float time, IValue<Vector2> range, IValue<Transform> target) : base(time)
        {
            this.range = range;
            this.target = target;
            
            map = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        }

        private IValue<Vector2> range;
        private IValue<Transform> target;
        
        private AnimationCurve map;

        protected override void Execute(Packet packet, float ratio)
        {
            if (range.IsValid(packet) && target.IsValid(packet))
            {
                var scale = Mathf.Lerp(range.Value.x, range.Value.y, map.Evaluate(Mathf.Clamp01(ratio)));
                target.Value.localScale = Vector3.one * scale;
            }
        }
    }
}