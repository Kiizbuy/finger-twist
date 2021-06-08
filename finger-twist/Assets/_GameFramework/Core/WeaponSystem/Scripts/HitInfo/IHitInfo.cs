using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public interface IHitInfo
    {
        Vector3 HitDirection { get; }
        Transform HitOwner { get; }
        float HitForce { get; }
    }
}
