using GameFramework.Components;
using GameFramework.Strategy;
using GameFramework.UnityEngine.EventsExtension;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.WeaponSystem
{
    public readonly struct AmmoInfoState
    {
        public readonly int AmmoCount;
        public readonly int AmmoClipCapacity;

        public AmmoInfoState(int ammoCount, int ammoClipCapacity)
        {
            AmmoCount = ammoCount;
            AmmoClipCapacity = ammoClipCapacity;
        }
    }

    [Serializable]
    public class AmmoChangedEvent : UnityEvent<AmmoInfoState> { }

    [RequireComponent(typeof(AudioSource))]
    public class GunWeapon : MonoBehaviour, IGunWeapon, IAttackable
    {
        [BoxGroup("UI Events")]
        public AmmoChangedEvent OnAmmoHasChanged;
        [BoxGroup("UI Events")]
        public UnityEventFloat OnReloading;
        [BoxGroup("UI Events")]
        public UnityEvent OnCantReload;
        [BoxGroup("UI Events")]
        public UnityEvent OnShootHasEmpty;

        [SerializeReference, StrategyContainer]
        public IShootType ShootType;

        [SerializeField]
        [BoxGroup("Visual Settings")]
        private AudioClip _shootSound;
        [SerializeField]
        [BoxGroup("Visual Settings")]
        private AudioClip _emptyShootSound;
        [SerializeField]
        [BoxGroup("Visual Settings")]
        private ParticleSystem _muzzleFlash;

        [SerializeField, Required]
        [BoxGroup("WeaponData")]
        private GunWeaponData _gunWeaponData;

        [SerializeField] [BoxGroup("Info")] private bool _infinityAmmo;

        [SerializeField]
        [BoxGroup("Info")]
        private int _ammoClipCapacity = 120;
        [SerializeField]
        [BoxGroup("Info")]
        private int _ammoCount = 120;

        public Coroutine ReloadCoroutineInstance { get; private set; }

        public AmmoInfoState CurrentAmmoInfoState => new AmmoInfoState(_ammoCount, _ammoClipCapacity);
        public int Damage => _gunWeaponData.Damage;
        public bool CanReload => _isReloading == false && (_ammoCount < _gunWeaponData.ReloadAmmoPerClipCapacity && _ammoClipCapacity > 0);
        public bool CanAttack() => _ammoCount > 0 && _isReloading == false;

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        private AudioSource _audioSource;
        private bool _isReloading;
        private float _currentReloadTimer;
        private float _currentFireTimer;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            if (_gunWeaponData == null)
                Debug.LogError("Gun weapon data is null", this);
        }

        private void OnDrawGizmos()
        {
            var gizmos = ShootType as IStrategyDrawGizmos;
            if (ShootType != null)
                gizmos?.DrawGizmos();
        }

        public void ChangeWeaponDataInfo(GunWeaponData value)
        {
            _gunWeaponData = value;
            _ammoClipCapacity = value.ReloadAmmoPerClipCapacity;
        }

        public void BreakReload()
        {
            if (ReloadCoroutineInstance != null)
                StopCoroutine(ReloadCoroutineInstance);
        }

        public void AddClipAmmoCapacity(int ammo)
        {
            _ammoClipCapacity = Mathf.Clamp(_ammoClipCapacity + ammo, 0, _gunWeaponData.MaxAmmoCapacity);
        }

        public void Attack()
        {
            if (CanAttack())
            {
                switch (_gunWeaponData.FireType)
                {
                    case FireType.Semi:
                        {
                            TryDecrementAmmoCount();
                            Fire();
                        }
                        break;
                    case FireType.Automatic:
                        {
                            _currentFireTimer += Time.deltaTime;

                            if (_currentFireTimer < _gunWeaponData.FireDelay)
                                return;
                            TryDecrementAmmoCount();
                            _currentFireTimer = 0f;
                            Fire();


                        }
                        break;
                    case FireType.LaserBeam:
                        {
                            Fire();
                            _currentFireTimer += Time.deltaTime;

                            if (_currentFireTimer < _gunWeaponData.FireDelay)
                                return;

                            TryDecrementAmmoCount();
                            _currentFireTimer = 0f;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                if (_emptyShootSound != null)
                    _audioSource.PlayOneShot(_emptyShootSound);

                OnShootHasEmpty?.Invoke();
            }
        }

        private void TryDecrementAmmoCount()
        {
            if (!_infinityAmmo)
                _ammoCount--;
        }

        public void StopAttack()
        {
            _currentFireTimer = 0f;
            ShootType.StopShoot();
        }

        private void Fire()
        {
            if (_shootSound != null)
                _audioSource.PlayOneShot(_shootSound);

            if (_muzzleFlash != null)
                _muzzleFlash.Play();

            ShootType.ShootAndTryTakeDamage(Damage, this);

            //if (_infinityAmmo == false)
            //    _ammoCount--;

            OnAmmoHasChanged?.Invoke(CurrentAmmoInfoState);
        }

        public void ReloadWeapon()
        {
            ReloadCoroutineInstance = StartCoroutine(ReloadCoroutine());
        }

        public void BreakReloadWeapon()
        {
            StopCoroutine(ReloadCoroutineInstance);
            _currentReloadTimer = 0f;
        }

        public IEnumerator ReloadCoroutine()
        {
            if (!CanReload)
            {
                OnCantReload?.Invoke();
                yield break;
            }

            _isReloading = true;
            _currentReloadTimer = 0f;

            while (_currentReloadTimer < _gunWeaponData.ReloadTime)
            {
                yield return _waitForFixedUpdate;
                _currentReloadTimer += Time.deltaTime;

                var normalizedReloadTime = _currentReloadTimer / _gunWeaponData.ReloadTime;

                OnReloading?.Invoke(normalizedReloadTime);


                if (_currentReloadTimer > _gunWeaponData.ReloadTime)
                {
                    var countToReload = _gunWeaponData.ReloadAmmoPerClipCapacity - _ammoCount;

                    if (_ammoClipCapacity < countToReload)
                    {
                        _ammoCount = countToReload;
                        _ammoClipCapacity = 0;
                    }
                    else
                    {
                        _ammoCount += countToReload;
                        _ammoClipCapacity -= countToReload;
                    }

                    _isReloading = false;
                    OnAmmoHasChanged?.Invoke(CurrentAmmoInfoState);
                }
            }
        }
    }
}
