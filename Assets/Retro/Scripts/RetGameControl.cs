using Flux;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGameControl : MonoBehaviour
    {
        [FoldoutGroup("Values"), SerializeField] private RetGame game;

        void Start() => Routines.Start(Routines.DoAfter(() => game.Begin(), new YieldFrame()));
    }
}