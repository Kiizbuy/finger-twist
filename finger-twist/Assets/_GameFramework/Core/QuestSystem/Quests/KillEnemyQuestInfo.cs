namespace GameFramework.Quest
{
    public readonly struct KillEnemyQuestInfo
    {
        public readonly string EnemyName;
        public readonly int MaxKilledEnemies;

        public KillEnemyQuestInfo(string enemyName, int maxKilledEnemies)
        {
            EnemyName = enemyName;
            MaxKilledEnemies = maxKilledEnemies;
        }
    }
}