namespace GameFramework.WeaponSystem
{
    public interface IWeapon
    {
        void Attack();
        void StopAttack();
        bool CanAttack();
        int Damage { get; }
    }
}
