using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;

namespace GameFramework.Quest
{
    public readonly struct CollectItemQuestInfo
    {
        public readonly string QuestItemId;
        public readonly int MaxItemsCountToCompleteQuest;

        public CollectItemQuestInfo(string questItemId, int maxItemsCountToCompleteQuest)
        {
            QuestItemId = questItemId;
            MaxItemsCountToCompleteQuest = maxItemsCountToCompleteQuest;
        }
    }

    public sealed class CollectItemsQuest : IQuest
    {
        public event Action<IQuest> OnStart;
        public event Action<IQuest> OnComplete;
        public event Action<IQuest> OnFailed;
        public event Action<IQuest, QuestStatus> OnStatusChanged;

        public QuestStatus CurrentQuestStatus { get; private set; }
        public IEnumerable<BaseItemData> RewardItems { get; }
        public string QuestName { get; }
        public int ExperienceReward { get; private set; }

        public int CurrentCollectableItemsCount
        {
            get => _currentCollectableItemsCount;
            private set
            {
                _currentCollectableItemsCount = value;
                EvaluateQuestCompletion();
            }
        }

        private int _currentCollectableItemsCount;

        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();
        private readonly CollectItemQuestInfo _collectItemQuestInfo;

        public CollectItemsQuest(string questName, CollectItemQuestInfo collectItemQuestInfo)
        {
            QuestName = questName;
            _collectItemQuestInfo = collectItemQuestInfo;
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
            if (questDto.ItemId == string.Empty)
                return;

            if (questDto.ItemId == _collectItemQuestInfo.QuestItemId)
                CurrentCollectableItemsCount++;

            ChangeQuestStatus(CurrentQuestStatus);
        }

        public void StartQuest()
        {
            if (CurrentQuestStatus != QuestStatus.InProgress || CurrentQuestStatus != QuestStatus.Complete)
                ChangeQuestStatus(QuestStatus.InProgress);

            OnStart?.Invoke(this);
        }

        public void CompleteQuest()
        {
            if (CurrentQuestStatus != QuestStatus.Complete)
                ChangeQuestStatus(QuestStatus.Complete);
            OnComplete?.Invoke(this);
        }

        public void FailQuest()
        {
            if (CurrentQuestStatus != QuestStatus.Failed)
                ChangeQuestStatus(QuestStatus.Failed);
            OnFailed?.Invoke(this);
        }

        public void EvaluateQuestCompletion()
        {
            if (CurrentQuestStatus == QuestStatus.NotStarted || CurrentQuestStatus == QuestStatus.Complete)
                return;

            if (CurrentCollectableItemsCount >= _collectItemQuestInfo.MaxItemsCountToCompleteQuest)
                CompleteQuest();
        }


        private void ChangeQuestStatus(QuestStatus status)
        {
            CurrentQuestStatus = status;
            OnStatusChanged?.Invoke(this, CurrentQuestStatus);
        }
    }
}
