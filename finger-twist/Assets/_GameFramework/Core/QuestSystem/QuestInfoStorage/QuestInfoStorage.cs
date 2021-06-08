using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Quest
{
    [CreateAssetMenu(menuName = "QuestSystem/Create quest info storage")]
    public class QuestInfoStorage : ScriptableObject
    {
        [ReorderableList]
        public List<string> AllQuestEnemiesNames = new List<string>();
        [ReorderableList]
        public List<string> AllQuestItemsNames = new List<string>();
    }
}
