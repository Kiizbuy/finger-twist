using NaughtyAttributes;
using NaughtyAttributes.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameFramework.Strategy
{
    [CustomPropertyDrawer(typeof(StrategyArrayContainerAttribute))]
    public class StrategyArrayContainerPropertyDrawer : SpecialCasePropertyDrawerBase
    {
        public new static readonly StrategyArrayContainerPropertyDrawer Instance = new StrategyArrayContainerPropertyDrawer();

        private readonly Dictionary<string, ReorderableList> _reorderableListsByPropertyName = new Dictionary<string, ReorderableList>();

        private string GetPropertyKeyName(SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetInstanceID() + "/" + property.name;
        }


        //private SerializedProperty _serializedProperty;
        //private IStrategyContainer _strategyFieldValue;
        //private FieldInfo _propertyField;
        //private int _strategyImplementationIndex;
        //private List<Type> _interfaceTypes;
        //private string[] _interfaceNames;



        //private void InitProperty(SerializedProperty property)
        //{
        //    var objectType = property.serializedObject.targetObject.GetType();

        //    _serializedProperty = property;
        //    //_propertyField = fieldInfo;
        //    _serializedProperty = property;
        //}

        protected override void OnGUI_Internal(SerializedProperty property, GUIContent label)
        {
            if (property.isArray)
            {
                string key = GetPropertyKeyName(property);

                if (!_reorderableListsByPropertyName.ContainsKey(key))
                {
                    ReorderableList reorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true)
                    {
                        drawHeaderCallback = (Rect rect) =>
                        {
                            EditorGUI.LabelField(rect, string.Format("{0}: {1}", label.text, property.arraySize), EditorStyles.boldLabel);
                        },

                        drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                        {
                            SerializedProperty element = property.GetArrayElementAtIndex(index);
                            rect.y += 1.0f;
                            rect.x += 10.0f;
                            rect.width -= 10.0f;

                            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 0.0f), element, true);
                        },

                        elementHeightCallback = (int index) =>
                        {
                            return EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index)) + 4.0f;
                        }
                    };

                    _reorderableListsByPropertyName[key] = reorderableList;
                }

                _reorderableListsByPropertyName[key].DoLayoutList();
            }
            else
            {
                string message = typeof(ReorderableListAttribute).Name + " can be used only on arrays or lists";
                NaughtyEditorGUI.HelpBox_Layout(message, MessageType.Warning, context: property.serializedObject.targetObject);
                EditorGUILayout.PropertyField(property, true);
            }
        }


        public void ClearCache()
        {
            _reorderableListsByPropertyName.Clear();
        }

        //private void InitializeInterfacesNames()
        //{
        //    _interfaceNames = new string[1];
        //    _interfaceNames[0] = "(None)";
        //    _interfaceNames = _interfaceNames.Concat(_interfaceTypes.Select(type => type.Name)).ToArray();
        //}

        private void ForeachChildProperty(SerializedProperty property, Action<SerializedProperty> action)
        {
            var endProperty = property.GetEndProperty();

            property = property.Copy();
            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, endProperty))
            {
                action(property);
                property.NextVisible(false);
            }
        }
    }
}

