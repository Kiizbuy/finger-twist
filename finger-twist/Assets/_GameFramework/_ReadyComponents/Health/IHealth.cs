namespace GameFramework.Components
{
    public interface IHealth
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        float PercentageOfHealth { get; }
        bool IsAlive { get; }
        bool IsImmortal { get; }
        void AddHealthPoint(int healthPoint);
        void TakeDamage(int damage, IAttackable attackable);
    }
}

