using UnityEngine;

namespace Chrome
{
    public interface IIntensityPassive
    {
        Vector2 Range { get; }

        void Bootup(IIdentity identity);
        
        void Start(float value, float ratio, IIdentity identity);
        void Update(float value, float ratio, IIdentity identity);
        void End(float value, float ratio, IIdentity identity);

        void Shutdown(IIdentity identity);
    }
}