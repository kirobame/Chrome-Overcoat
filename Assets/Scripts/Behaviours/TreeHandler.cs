using UnityEngine;

namespace Chrome
{
    public class TreeHandler : MonoBehaviour
    {
        private RootNode tree;
        private Packet packet;

        private bool isShuttingDown;
        
        void Awake()
        {
            isShuttingDown = false;
            packet = new Packet();
            
            tree = new RootNode().Append(
                new Print("Start of delay !").Append(
                    new Delay("1", 2.75f).Append(
                        new Print("End of delay."),
                        new Print("And another delay !").Append(
                            new Delay("2", 1.0f).Append(
                                new Print("End of another delay.")))))
            );
        }

        void Start() => tree.Start(packet);
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Wants to shutdown.");
                
                isShuttingDown = true;
                tree.Order(new ShutdownCommand());
            }

            if (isShuttingDown && !tree.IsLocked) return;
            tree.Update(packet);
        }
    }
}