//using GameFramework.Inventory.Items;
//using System;
//using System.Collections.Generic;

//namespace GameFramework.Quest
//{
//    public class SequenceQuest : IQuest
//    {
//        public event Action<IQuest> OnStart;
//        public event Action<IQuest> OnComplete;
//        public event Action<IQuest> OnFailed;
//        public event Action<IQuest, QuestStatus> OnStatusChanged;

//        private readonly List<IQuest> _childrenQuests = new List<IQuest>();
//        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();

//        public IEnumerable<BaseItemData> RewardItems => _rewardItems;

//        public QuestStatus CurrentQuestStatus { get; private set; }
//        public string QuestName { get; private set; }
//        public int ExperienceReward { get; private set; }
//        public bool QuestHasBeenComplete { get; private set; }

//        public SequenceQuest(string questName)
//        {
//            QuestName = questName;
//        }

//        public IQuest AddExperienceReward(int expPoints)
//        {
//            ExperienceReward = expPoints;
//            return this;
//        }

//        public IQuest AddRewardedItems(IEnumerable<BaseItemData> rewardItems)
//        {
//            return this;
//        }

//        public IQuest AddQuest(IQuest quest)
//        {
//            if (!_childrenQuests.Contains(quest))
//                _childrenQuests.Add(quest);

//            return this;
//        }

//        public void StartQuest()
//        {
//            throw new NotImplementedException();
//        }

//        public void AcceptToSubscribeEvent(IQuestViziter viziter)
//        {
//            throw new NotImplementedException();
//        }

//        public void AcceptToUnsubscribeEvent(IQuestViziter viziter)
//        {
//            throw new NotImplementedException();
//        }

//        public void CompleteQuest()
//        {
//            throw new NotImplementedException();
//        }

//        public void FailQuest()
//        {
//            throw new NotImplementedException();
//        }


//        public void EvaluateQuestCompletion()
//        {
//        }

//        public void Dispose()
//        {
//        }
//    }
//}
