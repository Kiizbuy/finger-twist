using UnityEngine;

namespace GameFramework.Quest
{
    public readonly struct KillQuestDTO : IQuestDTO
    {
        public Vector3 ReachTargetPosition { get; }
        public string EnemyId { get; }
        public string ItemId { get; }

        public KillQuestDTO(string enemyId)
        {
            ReachTargetPosition = default;
            ItemId = default;
            EnemyId = enemyId;
        }
    }
}
