using System;
using Flux;
using Flux.Event;
using Flux.Feedbacks;
using UnityEngine;

namespace Chrome
{
    [Serializable, Path("Select")]
    public class PointSelect : Effect
    {
        [SerializeField] private Transform source;
        [SerializeField] private float offset;
        
        protected override void OnUpdate(EventArgs args) => IsDone = true;

        protected override EventArgs OnTraversed(EventArgs args) => new WrapperArgs<Vector3>(source.position + offset * Vector3.up);
    }
}