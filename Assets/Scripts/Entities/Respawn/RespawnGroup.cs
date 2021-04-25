using UnityEngine;

namespace Chrome
{
    public class RespawnGroup : MonoBehaviour
    {
        [SerializeField] private string path;
        [SerializeField] private Transform[] values;

        void Awake() => Blackboard.Global.Set(path, values);
    }
}