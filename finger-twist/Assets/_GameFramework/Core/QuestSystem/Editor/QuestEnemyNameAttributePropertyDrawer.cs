using GameFramework.Utils.Editor;
using NaughtyAttributes.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Quest
{
    [CustomPropertyDrawer(typeof(QuestEnemyNameAttribute))]
    public class QuestEnemyNameAttributePropertyDrawer : PropertyDrawerBase
    {
        private QuestInfoStorage _questInfoStorage;

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            return (property.propertyType == SerializedPropertyType.String)
                ? GetPropertyHeight(property)
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_questInfoStorage == null)
                    _questInfoStorage = EditorUtils
                        .GetAllInstances<QuestInfoStorage>()
                        .FirstOrDefault();

                var propertyString = property.stringValue;
                var questItemNamesList = new List<string> { "(None)" };

                if (_questInfoStorage != null)
                    questItemNamesList.AddRange(_questInfoStorage.AllQuestEnemiesNames);

                var index = 0;

                if (questItemNamesList.Count == 0)
                    EditorGUILayout.HelpBox("ПОШЕЛ НАХУЙ, ЛИБО НЕ НАШЕЛ ТВОЕ ГОВНО, ЛИБО ИНФА ПУСТАЯ, ДОЛБОЕБИНА",
                        MessageType.Warning);

                for (var i = 1; i < questItemNamesList.Count; i++)
                {
                    if (questItemNamesList[i] != propertyString)
                        continue;

                    index = i;
                    break;
                }

                index = EditorGUI.Popup(rect, label.text, index, questItemNamesList.ToArray());
                property.stringValue = index > 0 ? questItemNamesList[index] : string.Empty;
            }
            else
            {
                var message = $"{nameof(QuestItemNameAttributePropertyDrawer)} supports only string fields";
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }
    }
}
