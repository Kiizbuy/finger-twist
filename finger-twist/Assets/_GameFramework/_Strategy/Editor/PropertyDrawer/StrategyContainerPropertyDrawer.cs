using GameFramework.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Strategy
{
    [CustomPropertyDrawer(typeof(StrategyContainerAttribute))]
    public class StartegyAttributePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _serializedProperty;
        private IStrategyContainer _strategyFieldValue;
        private FieldInfo _propertyField;
        private List<Type> _strategyImplementationTypes;
        private int _strategyImplementationIndex;
        private string[] _strategyImplementationNames;

        private void InitProperty(SerializedProperty property)
        {
            var objectType = property.serializedObject.targetObject.GetType();
            _serializedProperty = property;
            _propertyField = objectType.GetField(property.name);
            _strategyFieldValue = (IStrategyContainer)_propertyField.GetValue(property.serializedObject.targetObject);
            _serializedProperty = property;
        }

        private void InitializeStrategyContainerImplementations()
        {
            if (_strategyImplementationTypes == null)
                _strategyImplementationTypes = ReflectionUtils.TryGetStrategyImplementationsFromType(_propertyField.FieldType);
        }

        private void InitializeInterfacesNames()
        {
            _strategyImplementationNames = new string[1];
            _strategyImplementationNames[0] = "(None)";
            _strategyImplementationNames = _strategyImplementationNames.Concat(_strategyImplementationTypes.Select(x => x.Name)).ToArray();
        }


        private GUIStyle GetLabelGUIStyle()
        {
            var guiStyle = new GUIStyle();

            guiStyle.fontStyle = FontStyle.Bold;
            guiStyle.richText = false;
            guiStyle.fixedWidth = 200;

            return guiStyle;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitProperty(property);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.BeginVertical("Toolbar");

            DrawpopupStrategyImplementations(property, label);

            EditorGUILayout.EndVertical();

            DrawStrategyImplementationFields(position, property);

            CheckAndChangeStrategyTypeState();

            EditorGUILayout.EndVertical();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
        }

        private void DrawStrategyImplementationFields(Rect position, SerializedProperty property)
        {
            if (_strategyFieldValue == null)
                return;

            EditorGUI.indentLevel++;

            ForeachChildProperty(property, (prop) =>
                EditorGUILayout.PropertyField(prop, new GUIContent(prop.name), true, null));

            EditorGUI.indentLevel--;
        }

        private void DrawpopupStrategyImplementations(SerializedProperty property, GUIContent label)
        {
            if (_propertyField == null)
                return;

            if (_strategyImplementationTypes == null)
                InitializeStrategyContainerImplementations();


            if (_strategyImplementationTypes == null)
            {
                EditorGUILayout.HelpBox("Not found strategy implementations", MessageType.Warning);
                return;
            }

            if (_strategyImplementationNames == null)
                InitializeInterfacesNames();

            _strategyImplementationIndex = 0;

            if (_strategyFieldValue != null)
                _strategyImplementationIndex = 1 + _strategyImplementationTypes.FindIndex(0, type => type == _strategyFieldValue.GetType());

            var defaultLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 10;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GetLabelGUIStyle());

            _strategyImplementationIndex = EditorGUILayout.Popup(_strategyImplementationIndex, _strategyImplementationNames);

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = defaultLabelWidth;

        }


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

        private void CheckAndChangeStrategyTypeState()
        {
            if (_propertyField == null)
                return;

            if (_strategyImplementationIndex == 0)
            {
                ClearStrategy();
                return;
            }

            if (_strategyFieldValue == null || _strategyFieldValue.GetType() != _strategyImplementationTypes[_strategyImplementationIndex - 1])
                CreateStrategy(_strategyImplementationTypes[_strategyImplementationIndex - 1]);
        }

        private void ClearStrategy()
        {
            if (_strategyFieldValue != null)
            {
                _serializedProperty.managedReferenceValue = null;
                _serializedProperty.serializedObject.ApplyModifiedProperties();
                Debug.Log("Clear");
                _strategyImplementationIndex = 0;
            }
        }

        private void CreateStrategy(Type strategyType)
        {
            var strategyImplementation = Activator.CreateInstance(strategyType) as IStrategyContainer;

            _strategyFieldValue = strategyImplementation;
            _serializedProperty.managedReferenceValue = _strategyFieldValue;
            _serializedProperty.serializedObject.ApplyModifiedProperties();

        }
    }
}
