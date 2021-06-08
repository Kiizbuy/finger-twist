using UnityEngine;

namespace GameFramework.Quest
{
    public readonly struct ReachTargetQuestDTO : IQuestDTO
    {
        public Vector3 ReachTargetPosition { get; }
        public string EnemyId { get; }
        public string ItemId { get; }

        public ReachTargetQuestDTO(Vector3 reachTargetPosition)
        {
            ReachTargetPosition = reachTargetPosition;
            ItemId = default;
            EnemyId = default;
        }
    }
}
