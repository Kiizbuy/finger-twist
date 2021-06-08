using System.Collections.Generic;

namespace GameFramework.Quest
{
    public interface IQuestTrigger<in TQuestData> where TQuestData : IQuestDTO
    {
        void Trigger(IEnumerable<IQuest> quests, TQuestData questData);
    }
}
