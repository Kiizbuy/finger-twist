using System.Reflection;
using UnityEditor;
using UnityEditorExtensions;
using UnityEditorInternal;
using UnityEngine;

namespace GameFramework.Events
{
    [CustomPropertyDrawer(typeof(MethodToEventSubscribeContainer))]
    public class MethodToEventSubscribeContainerPropertyDrawer : PropertyDrawer
    {
        private readonly Color headerColor = new Color(1, 0.45f, 0);
        private ReorderableList methodsTemplateDataReorderableList;
        private MethodToEventSubscribeContainer propertyObjectInstance;

        private SerializedProperty eventsTemplateDataProperty;

        private Rect reordableListRect;
        private Rect foldoutButtonRect;
        private Rect foldoutLabelRect;
        private Rect foldinBoxRect;

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
            reordableListRect = propertyRect;
            foldoutButtonRect = propertyRect;
            foldoutLabelRect = propertyRect;
            foldinBoxRect = propertyRect;

            foldinBoxRect.xMin = propertyRect.x + 2;

            foldoutButtonRect.width = 100;
            foldoutButtonRect.height = 20;

            foldoutLabelRect.x += 6;
            foldoutLabelRect.width = 400;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectWhoUseProperty = property.GetObjectWhoUseTheProperty();
            var methodNameAttribute = (MethodNameAttribute)fieldInfo.GetCustomAttribute(typeof(MethodNameAttribute), false);
            var methodName = string.Empty;

            if (methodNameAttribute == null)
            {
                Debug.LogError("Method name attribute is not found. Mark it");
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, "Method name attribute is not found. Mark it", GetHeaderGUIStyle(Color.red));
                return;
            }

            methodName = methodNameAttribute.MethodName;
            eventsTemplateDataProperty = property.FindPropertyRelative("_eventsTemplateData");
            propertyObjectInstance = property.GetObjectValueFromSerializedProperty<MethodToEventSubscribeContainer>();

            if (ObjectDoesntHaveMethod(objectWhoUseProperty, methodName))
            {
                Debug.LogError($"{methodName} event doesn't exsist on object, Please, use an existing eventName on object if you have it");
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, $"''{methodName}'' event doesn't exsist on object", GetHeaderGUIStyle(Color.red));
                return;
            }

            InitializeRects(position);

            property.isExpanded = EditorGUI.Foldout(foldoutButtonRect, property.isExpanded, GUIContent.none, true);

            if (methodsTemplateDataReorderableList == null)
                methodsTemplateDataReorderableList = BuildReorderableListFromProperty(eventsTemplateDataProperty, methodName);

            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (property.isExpanded)
            {
                methodsTemplateDataReorderableList.DoList(reordableListRect);
            }
            else
            {
                var headerGUIContent = new GUIContent(methodName);
                GUI.Box(foldinBoxRect, GUIContent.none);
                GUI.Label(foldoutLabelRect, headerGUIContent, GetHeaderGUIStyle(headerColor));
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private bool ObjectDoesntHaveMethod(object eventableObject, string methodName)
        {
            return eventableObject.GetType().GetMethod(methodName) == null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return GetPropertyHeightFromReorderableList(methodsTemplateDataReorderableList);

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
                },

            };

            newReordableList.drawHeaderCallback += (rect) =>
            {
                var headerGUIContent = new GUIContent(headerName);
                GUI.Label(rect, headerGUIContent, GetHeaderGUIStyle(headerColor));
            };

            newReordableList.elementHeight = 40f;

            return newReordableList;
        }
    }
}

