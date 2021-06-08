using UnityEngine;

namespace GameFramework.Quest
{
    public class DummyQuestEnemy : MonoBehaviour
    {
        private QuestHandler _questHandler;
        private KillQuestTrigger _killQuestTrigger;

        public void DeadEnemy()
        {
            EnemyHasDied = true;
        }

        public string EnemyName => "Dummy";
        public int EnemyExperienceReward => 1;
        public int EnemyLevel => 1;
        public bool EnemyHasDied { get; private set; }
    }
}
