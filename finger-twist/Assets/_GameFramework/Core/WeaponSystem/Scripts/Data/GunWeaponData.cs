using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public enum FireType
    {
        Semi,
        Automatic,
        LaserBeam,
    }

    [CreateAssetMenu(menuName = "Weapon/Gun/Create weapon data", fileName = "GunWeaponData")]
    public class GunWeaponData : WeaponData
    {
        [SerializeField] private FireType _fireType;
        [SerializeField] private float _fireDelay;
        [SerializeField] private float _reloadTime;
        [SerializeField] private int _maxAmmoCapacity;
        [SerializeField] private int _reloadAmmoPerClipCapacity;

        public FireType FireType => _fireType;
        public float FireDelay => _fireDelay;
        public float ReloadTime => _reloadTime;
        public int MaxAmmoCapacity => _maxAmmoCapacity;
        public int ReloadAmmoPerClipCapacity => _reloadAmmoPerClipCapacity;
    }
}
