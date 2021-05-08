using Flux;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPlayerLife : MonoBehaviour, ILink<IIdentity>
    {
        public IIdentity Identity => identity;
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        [FoldoutGroup("Dependencies"), SerializeField] private CharacterController controller;
        [FoldoutGroup("Dependencies"), SerializeField] private Lifetime lifetime;
        
        [FoldoutGroup("Values"), SerializeField] protected string path;
        [FoldoutGroup("Values"), SerializeField] protected int index;

        void Start() => Routines.Start(Routines.DoAfter(() => lifetime.End(), new YieldFrame()));
        
        public void Bootup()
        {
            var respawnGroup = Blackboard.Global.Get<Transform[]>(path);
            transform.root.position = respawnGroup[index].position;

            controller.enabled = true;
        }
        
        public void Shutdown()
        {
            var gameScreen = Repository.Get<RetGameControl>(RetReference.Game);
            gameScreen.SwitchTo(RetGameState.Lost);

            controller.enabled = false;
        }
        
        public void Hit(IIdentity source, float amount, Packet packet)
        {
            var sourceType = source.Packet.Get<byte>();
            Events.ZipCall<float,byte>(GaugeEvent.OnDamageReceived, amount, sourceType);
        }
    }
}