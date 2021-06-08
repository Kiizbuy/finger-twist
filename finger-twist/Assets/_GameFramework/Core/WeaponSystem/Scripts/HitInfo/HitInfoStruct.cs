using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public readonly struct HitInfoStruct : IHitInfo
    {
        public Vector3 HitDirection { get; }
        public Transform HitOwner { get; }
        public float HitForce { get; }

        public HitInfoStruct(Vector3 hitDirection, Transform hitOwner, float hitForce)
        {
            HitDirection = hitDirection;
            HitOwner = hitOwner;
            HitForce = hitForce;
        }
    }
}
