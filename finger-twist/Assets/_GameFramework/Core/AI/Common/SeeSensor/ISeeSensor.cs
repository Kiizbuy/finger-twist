using UnityEngine;

namespace GameFramework.AI
{
    public interface ISeeSensor
    {
        float SeeRange { get; }
        float SeeAngle { get; }
        bool CanSee { get; }
        Transform SeeSensorPoint { get; }
        bool SeeTarget(LayerMask seeLayerMask, out Transform seenTransform);
    }
}
