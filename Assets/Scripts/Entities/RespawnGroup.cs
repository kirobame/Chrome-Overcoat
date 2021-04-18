using UnityEngine;

namespace Chrome
{
    public class RespawnGroup : MonoBehaviour
    {
        [SerializeField] private string path;
        [SerializeField] private Transform[] values;

        void Awake()
        {
            Debug.Log($"Setting : {path} - {values[0]}");
            Blackboard.Global.Set(values, path);
        }
    }
}