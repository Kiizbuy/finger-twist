using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Quest
{
    [RequireComponent(typeof(Rigidbody))]
    public class ReachTargetQuestTrigger : MonoBehaviour, IQuestTrigger<ReachTargetQuestDTO>
    {
        public QuestHandler QuestHandler;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider collider)
        {
            Trigger(QuestHandler.GetAllQuests(), new ReachTargetQuestDTO(transform.position));
        }

        public void Trigger(IEnumerable<IQuest> quests, ReachTargetQuestDTO questData)
        {
            foreach (var quest in quests)
            {
                quest.ApplyQuestDTO(questData);
            }
        }
    }
}
