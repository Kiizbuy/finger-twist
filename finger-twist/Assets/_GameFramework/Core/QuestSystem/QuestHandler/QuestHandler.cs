using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Quest
{
    public class QuestHandler : MonoBehaviour
    {
        public Action<IQuest> OnQuestStarted;
        public Action<IQuest> OnQuestAdded;
        public Action<IQuest> OnQuestComplete;
        public Action<IQuest> OnQuestFailed;
        public Action<IQuest, QuestStatus> OnQuestStatusHasChanged;

        private readonly List<IQuest> _allQuests = new List<IQuest>();

        public IEnumerable<IQuest> GetAllQuests() => _allQuests;
        public IEnumerable<IQuest> GetAllQuests(QuestStatus status) => _allQuests.FindAll(x => x.CurrentQuestStatus == status);

        public void TryAddQuest(IQuest quest)
        {
            if (!_allQuests.Contains(quest))
                _allQuests.Add(quest);

            quest.OnStart += QuestStartHandler;
            quest.OnStatusChanged += QuestStatusChangeHandler;
            quest.OnComplete += QuestCompleteHandler;
            quest.OnFailed += QuestFailureHandler;
            quest.EvaluateQuestCompletion();

            OnQuestAdded?.Invoke(quest);
        }

        public void StartQuest(IQuest quest)
        {
            if (_allQuests.Contains(quest))
                quest.StartQuest();
        }

        public void FailQuest(IQuest quest)
        {
            if (_allQuests.Contains(quest))
                quest.FailQuest();
        }

        public void QuestStatusChangeHandler(IQuest quest, QuestStatus status)
            => OnQuestStatusHasChanged?.Invoke(quest, status);

        public void QuestCompleteHandler(IQuest quest)
        {
            OnQuestComplete?.Invoke(quest);
        }
        public void QuestFailureHandler(IQuest quest)
        {
            OnQuestFailed?.Invoke(quest);
        }

        public void QuestStartHandler(IQuest quest)
        {
            OnQuestStarted?.Invoke(quest);
        }
    }
}
