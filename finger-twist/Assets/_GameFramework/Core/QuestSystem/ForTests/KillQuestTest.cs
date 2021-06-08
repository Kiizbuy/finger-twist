using UnityEngine;

namespace GameFramework.Quest
{
    public class KillQuestTest : MonoBehaviour
    {
        public DummyQuestEnemy DummyQuestEnemy;
        public QuestHandler QuestHandler;

        private void Start()
        {
            QuestHandler.OnQuestStarted += (quest) => Debug.Log($"{quest.QuestName} started");
            QuestHandler.OnQuestAdded += (quest) => Debug.Log($"Added new quest {quest.QuestName}");
            QuestHandler.OnQuestStatusHasChanged += (quest, status) =>
                Debug.Log($"quest {quest.QuestName} changed status - {quest.CurrentQuestStatus}");
            QuestHandler.OnQuestComplete += (quest) => Debug.Log($"Quest {quest.QuestName} has been completed");

            var killQuest = new KillQuest("Kill Dummy questEnemy", new KillEnemyQuestInfo(DummyQuestEnemy.EnemyName, 2)).AddExperienceReward(20);
            QuestHandler.TryAddQuest(killQuest);
            QuestHandler.StartQuest(killQuest);
        }
    }
}
