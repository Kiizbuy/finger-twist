using UnityEngine;

namespace GameFramework.Quest
{
    public interface IQuestDTO
    {
        Vector3 ReachTargetPosition { get; }
        string EnemyId { get; }
        string ItemId { get; }
    }
}
