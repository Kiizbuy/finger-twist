using GameFramework.Components;
using GameFramework.Strategy;

namespace GameFramework.WeaponSystem
{
    public interface IShootType : IStrategyContainer, IStrategyDrawGizmos
    {
        void ShootAndTryTakeDamage(int damage, IAttackable attackable);
        void StopShoot();
    }
}
