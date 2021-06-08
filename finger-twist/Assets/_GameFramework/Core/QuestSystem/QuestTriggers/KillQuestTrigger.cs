using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameFramework.Quest
{
    [Serializable]
    public class Huy
    {
        [QuestEnemyName]
        public string Huyy;
    }

    public class KillQuestTrigger : MonoBehaviour, IQuestTrigger<KillQuestDTO>
    {
        public QuestHandler QuestHandler;
        public Huy Huy;
        [QuestEnemyName]
        public string EnemyName;

        public void InvokeTrigger()
        {
            Trigger(QuestHandler.GetAllQuests(), new KillQuestDTO(EnemyName));
        }


        public void Trigger(IEnumerable<IQuest> quests, KillQuestDTO questData)
        {
            foreach (var quest in quests)
            {
                quest.ApplyQuestDTO(questData);
            }
        }
    }
}
