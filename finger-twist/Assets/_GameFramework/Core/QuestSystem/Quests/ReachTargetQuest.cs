using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Quest
{
    public class ReachTargetQuest : IQuest
    {
        public event Action<IQuest> OnStart;
        public event Action<IQuest> OnComplete;
        public event Action<IQuest> OnFailed;
        public event Action<IQuest, QuestStatus> OnStatusChanged;

        public QuestStatus CurrentQuestStatus { get; private set; }
        public IEnumerable<BaseItemData> RewardItems { get; }
        public string QuestName { get; }
        public int ExperienceReward { get; private set; }
        public bool QuestHasBeenComplete { get; }

        private Vector3 ActualPosition
        {
            get => _actualPosition;
            set
            {
                _actualPosition = value;
                EvaluateQuestCompletion();
            }
        }

        private Vector3 _actualPosition;
        private readonly Vector3 _reachPosition;
        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();

        public ReachTargetQuest(string questName, Vector3 reachPosition)
        {
            QuestName = questName;
            _reachPosition = reachPosition;
        }

        public IQuest AddExperienceReward(int expPoints)
        {
            ExperienceReward = expPoints;
            return this;
        }

        public IQuest AddRewardedItems(IEnumerable<BaseItemData> rewardItems)
        {
            _rewardItems.AddRange(rewardItems);
            return this;
        }


        public void ApplyQuestDTO(IQuestDTO questDto)
        {
            ActualPosition = questDto.ReachTargetPosition;
        }

        public void StartQuest()
        {
            ChangeQuestStatus(QuestStatus.InProgress);
            OnStart?.Invoke(this);
        }

        public void CompleteQuest()
        {
            ChangeQuestStatus(QuestStatus.Complete);
            OnComplete?.Invoke(this);
        }

        public void FailQuest()
        {
            ChangeQuestStatus(QuestStatus.Failed);
            OnFailed?.Invoke(this);
        }

        public void EvaluateQuestCompletion()
        {
            if (CurrentQuestStatus == QuestStatus.NotStarted || CurrentQuestStatus == QuestStatus.Complete)
                return;

            if ((ActualPosition - _reachPosition).sqrMagnitude < 0.001f)
                CompleteQuest();
        }

        private void ChangeQuestStatus(QuestStatus status)
        {
            CurrentQuestStatus = status;
            OnStatusChanged?.Invoke(this, CurrentQuestStatus);
        }
    }
}
