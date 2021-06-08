using GameFramework.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.WeaponSystem
{
    public class ProjectileShoot : IShootType
    {
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private Projectile _projectileModel;
        [SerializeField] private PhysicInteractionType _physicInteractionType;
        [SerializeField] private float _shootForce = 5f;

        public void ShootAndTryTakeDamage(int damage, IAttackable attackable)
        {

            var projectileParameters = new ProjectileDataInfo(_physicInteractionType, _projectileSpawnPoint.forward, _shootForce, damage);
            var projectile = Object.Instantiate(_projectileModel, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);

            projectile.PushProjectile(projectileParameters);
        }

        public void StopShoot()
        {
        }

        public void DrawGizmos()
        {
            if (_projectileSpawnPoint == null)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_projectileSpawnPoint.position, _projectileSpawnPoint.position + _projectileSpawnPoint.forward * _shootForce);
        }
    }
}
