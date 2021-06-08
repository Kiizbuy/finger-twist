using UnityEngine;

namespace GameFramework.WeaponSystem
{
    public readonly struct ProjectileViewMoverInfo
    {
        public readonly float DistanceToDestroy;
        public readonly float FlySpeed;

        public ProjectileViewMoverInfo(float distanceToDestroy, float flySpeed)
        {
            DistanceToDestroy = distanceToDestroy;
            FlySpeed = flySpeed;
        }
    }

    public class ProjectileViewMover : MonoBehaviour
    {
        [SerializeField] private float _liveTime = 5f;

        private Transform _transform;
        private float _distanceToDestroy;
        private float _flySpeed;
        private float _spawnTime;
        private bool _canFly;

        public void PushProjectileView(ProjectileViewMoverInfo projectileViewInfo)
        {
            _distanceToDestroy = projectileViewInfo.DistanceToDestroy;
            _flySpeed = projectileViewInfo.FlySpeed;
            _canFly = true;
        }

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _spawnTime = Time.time;
        }

        private void FixedUpdate()
        {
            if (!_canFly)
                return;

            Fly();
        }

        private void Fly()
        {
            _transform.position += _transform.forward * _flySpeed * Time.deltaTime;
            _distanceToDestroy -= _flySpeed * Time.deltaTime;

            if (Time.time > _spawnTime + _liveTime || _distanceToDestroy < 0)
                Destroy(gameObject);
        }
    }
}
