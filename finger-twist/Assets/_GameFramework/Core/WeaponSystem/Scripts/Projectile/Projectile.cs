using GameFramework.Components;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.WeaponSystem
{
    public enum PhysicInteractionType
    {
        Collider,
        Trigger
    };

    public struct ProjectileDataInfo
    {
        public readonly PhysicInteractionType PhysicInteractionType;
        public readonly Vector3 Direction;
        public readonly float Force;
        public readonly int Damage;

        public ProjectileDataInfo(PhysicInteractionType physicInteractionType, Vector3 direction, float force, int damage)
        {
            PhysicInteractionType = physicInteractionType;
            Direction = direction;
            Force = force;
            Damage = damage;
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IAttackable
    {
        public UnityEvent OnProjectileHasDead;

        [SerializeField] private float _liveTime = 5f;

        private Rigidbody _rigidbody;
        private PhysicInteractionType _physicInteractionType;
        private int _damage;
        private float _spawnTime;

        public void PushProjectile(ProjectileDataInfo projectileInfo)
        {
            _damage = projectileInfo.Damage;
            _physicInteractionType = projectileInfo.PhysicInteractionType;
            _rigidbody.AddForce(projectileInfo.Direction * projectileInfo.Force, ForceMode.Impulse);
        }

        private void OnEnable()
        {
            _spawnTime = Time.time;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (Time.time > _spawnTime + _liveTime)
                DestroyProjectile();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_physicInteractionType == PhysicInteractionType.Trigger)
                TryTakeDamage(other.GetComponent<IHealth>());
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_physicInteractionType == PhysicInteractionType.Collider)
                TryTakeDamage(collision.transform.GetComponent<IHealth>());
        }

        private void TryTakeDamage(IHealth damagable)
        {
            if (damagable != null)
            {
                damagable.TakeDamage(_damage, this);
                DestroyProjectile();
            }
        }

        private void DestroyProjectile()
        {
            OnProjectileHasDead?.Invoke();
            Destroy(gameObject);
        }
    }

}
