using UnityEngine;

namespace GameFramework.Quest
{
    public class ReachTargetQuestTest : MonoBehaviour
    {
        public Transform ReachTargetPoint;
        public QuestHandler QuestHandler;

        private void Start()
        {
            var reachTargetQuest = new ReachTargetQuest("Reach Target", ReachTargetPoint.position).AddExperienceReward(20);
            QuestHandler.TryAddQuest(reachTargetQuest);
            QuestHandler.StartQuest(reachTargetQuest);
        }
    }
}
