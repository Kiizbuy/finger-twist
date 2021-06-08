using System.Collections;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public interface IGunWeapon : IWeapon
    {
        Coroutine ReloadCoroutineInstance { get; }
        AmmoInfoState CurrentAmmoInfoState { get; }
        IEnumerator ReloadCoroutine();
        bool CanReload { get; }
        void ReloadWeapon();
    }
}
