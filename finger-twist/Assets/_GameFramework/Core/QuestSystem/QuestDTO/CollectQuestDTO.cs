using UnityEngine;

namespace GameFramework.Quest
{
    public readonly struct CollectQuestDTO : IQuestDTO
    {
        public Vector3 ReachTargetPosition { get; }
        public string EnemyId { get; }
        public string ItemId { get; }

        public CollectQuestDTO(string itemId)
        {
            ItemId = itemId;
            ReachTargetPosition = default;
            EnemyId = default;
        }
    }
}
