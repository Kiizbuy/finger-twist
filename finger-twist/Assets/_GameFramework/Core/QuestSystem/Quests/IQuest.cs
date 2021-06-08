using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;

namespace GameFramework.Quest
{
    public interface IQuest
    {
        event Action<IQuest> OnStart;
        event Action<IQuest> OnComplete;
        event Action<IQuest> OnFailed;
        event Action<IQuest, QuestStatus> OnStatusChanged;
        IQuest AddExperienceReward(int expPoints);
        IQuest AddRewardedItems(IEnumerable<BaseItemData> rewardItems);
        QuestStatus CurrentQuestStatus { get; }
        IEnumerable<BaseItemData> RewardItems { get; }
        string QuestName { get; }
        int ExperienceReward { get; }
        void ApplyQuestDTO(IQuestDTO questDto);
        void StartQuest();
        void CompleteQuest();
        void FailQuest();
        void EvaluateQuestCompletion();
    }
}
