using System.Reflection;
using UnityEditor;
using UnityEditorExtensions;
using UnityEditorInternal;
using UnityEngine;

namespace GameFramework.Events
{
    [CustomPropertyDrawer(typeof(EventToMethodSubscribeСontainer))]
    public class EventToMethodSubscribeСontainerPropertyDrawer : PropertyDrawer
    {
        private ReorderableList _methodsTemplateDataReorderableList;
        private EventToMethodSubscribeСontainer _propertyObjectInstance;

        private SerializedProperty _methodsTemplateDataProperty;

        private Rect _reordableListRect;
        private Rect _foldoutButtonRect;
        private Rect _foldoutLabelRect;
        private Rect _foldinBoxRect;

        private readonly string _methodsTemplateDataLabel = "_methodsTemplateData";
        private readonly string _eventWarrningMessageLabel = "Event name attribute is not found. Mark it";

        private GUIStyle GetHeaderGUIStyle(Color labelColor)
        {
            var labelGUIStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };

            labelGUIStyle.normal.textColor = labelColor;

            return labelGUIStyle;
        }


        private void InitializeRects(Rect propertyRect)
        {
            _reordableListRect = propertyRect;
            _foldoutButtonRect = propertyRect;
            _foldoutLabelRect = propertyRect;
            _foldinBoxRect = propertyRect;

            _foldinBoxRect.xMin = propertyRect.x + 2;

            _foldoutButtonRect.width = 100;
            _foldoutButtonRect.height = 20;

            _foldoutLabelRect.x += 6;
            _foldoutLabelRect.width = 400;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectWhoUseProperty = property.GetObjectWhoUseTheProperty();
            var eventNameAttribute = (EventNameAttribute)fieldInfo.GetCustomAttribute(typeof(EventNameAttribute), false);
            var eventName = string.Empty;

            if (eventNameAttribute == null)
            {
                Debug.LogError(_eventWarrningMessageLabel);
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, _eventWarrningMessageLabel, GetHeaderGUIStyle(Color.red));
                return;
            }

            eventName = eventNameAttribute.EventName;
            _methodsTemplateDataProperty = property.FindPropertyRelative(_methodsTemplateDataLabel);
            _propertyObjectInstance = property.GetObjectValueFromSerializedProperty<EventToMethodSubscribeСontainer>();

            if (ObjectDoesntHaveEvent(objectWhoUseProperty, eventName))
            {
                Debug.LogError($"{eventName} event doesn't exsist on object, Please, use an existing eventName on object if you have it");
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, $"''{eventName}'' event doesn't exsist on object", GetHeaderGUIStyle(Color.red));
                return;
            }

            InitializeRects(position);

            property.isExpanded = EditorGUI.Foldout(_foldoutButtonRect, property.isExpanded, GUIContent.none, true);

            if (_methodsTemplateDataReorderableList == null)
                _methodsTemplateDataReorderableList = BuildReorderableListFromProperty(_methodsTemplateDataProperty, eventName);

            EditorGUI.BeginProperty(position, label, property);


            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (property.isExpanded)
            {
                _methodsTemplateDataReorderableList.DoList(_reordableListRect);
            }
            else
            {
                var headerGUIContent = new GUIContent(eventName);
                GUI.Box(_foldinBoxRect, GUIContent.none);
                GUI.Label(_foldoutLabelRect, headerGUIContent, GetHeaderGUIStyle(Color.cyan));
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private bool ObjectDoesntHaveEvent(object eventableObject, string eventName)
        {
            return eventableObject.GetType().GetEvent(eventName) == null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return GetPropertyHeightFromReorderableList(_methodsTemplateDataReorderableList);

            return EditorGUIUtility.singleLineHeight;
        }

        private float GetPropertyHeightFromReorderableList(ReorderableList list)
        {
            return (list != null ?
                    list.headerHeight + list.footerHeight + (list.elementHeight * Mathf.Max(list.count, 1) + 10f) :
                    80);
        }

        private ReorderableList BuildReorderableListFromProperty(SerializedProperty property, string headerName)
        {
            var newReordableList = new ReorderableList(property.serializedObject, property, true, false, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), true);
                }
            };

            newReordableList.drawHeaderCallback += (rect) =>
            {
                var headerGUIContent = new GUIContent(headerName);
                GUI.Label(rect, headerGUIContent, GetHeaderGUIStyle(Color.cyan));
            };

            newReordableList.elementHeight = 40f;

            return newReordableList;
        }
    }
}