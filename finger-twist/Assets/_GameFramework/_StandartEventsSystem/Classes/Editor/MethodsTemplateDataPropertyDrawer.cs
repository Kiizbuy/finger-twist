using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Events
{
    [CustomPropertyDrawer(typeof(MethodsTemplateData))]
    public class MethodsTemplateDataPropertyDrawer : PropertyDrawer
    {
        private GUIContent _selectedMonobehaviourMethodInfoTitle = new GUIContent("None");

        private SerializedProperty _mainProperty;
        private SerializedProperty _monobehaviourMethodNameProperty;
        private SerializedProperty _monobehaviourReferenceProperty;
        private SerializedProperty _eventObjectProperty;
        private SerializedProperty _isGlobalEventProperty;
        private SerializedProperty _globalEventNameProperty;

        private Rect _eventObjectRect;
        private Rect _isGlobalEventRect;
        private Rect _methodsDropdownButtonRect;
        private Rect _eventObjectRectTitle;
        private Rect _isGlobalEventRectTitle;
        private Rect _methodsDropdownButtonRectTitle;
        private Rect _globalEventNameTitleRect;

        private readonly string _monobehaviourMethodPropertyName = "_monobehaviourMethodName";
        private readonly string _monobehaviourReferencePropertyName = "_monobehaviourReference";
        private readonly string _eventObjectPropertyName = "_eventObject";
        private readonly string _isGlobalEventPropertyName = "_isGlobalEvent";
        private readonly string _globalEventPropertyName = "_globalEventName";
        private readonly string _eventObjectLabel = "Event Object";
        private readonly string _isGlobalEventLabel = "Global event?";
        private readonly string _globalEventNameLabel = "Global event name";
        private readonly string _avaliableMethodsLabel = "Avaliable methods";
        private readonly string _warningTitle = "Warning";
        private readonly string _nothingFoundMessage = "Nothing found";
        private readonly string _okLabel = "OK";
        private readonly string _noneLabel = "None";

        private GUIStyle GetSlowTextStyle()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = 8,
                fontStyle = FontStyle.Bold,
                fixedWidth = 100
            };

            return style;
        }

        private void InitializeProperties(SerializedProperty propertyFromRootPropertyObject)
        {
            _mainProperty = propertyFromRootPropertyObject;

            _monobehaviourMethodNameProperty = propertyFromRootPropertyObject.FindPropertyRelative(_monobehaviourMethodPropertyName);
            _monobehaviourReferenceProperty = propertyFromRootPropertyObject.FindPropertyRelative(_monobehaviourReferencePropertyName);
            _eventObjectProperty = propertyFromRootPropertyObject.FindPropertyRelative(_eventObjectPropertyName);
            _isGlobalEventProperty = propertyFromRootPropertyObject.FindPropertyRelative(_isGlobalEventPropertyName);
            _globalEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative(_globalEventPropertyName);
        }

        private void InitializePropertyRects(Rect propertyRect)
        {
            _eventObjectRect = new Rect(propertyRect.x, propertyRect.y, 100, 20);
            _isGlobalEventRect = new Rect(propertyRect.x + 105, propertyRect.y, 50, 20);
            _methodsDropdownButtonRect = new Rect(propertyRect.x + 175, propertyRect.y, propertyRect.width - 220, 20);
            _eventObjectRectTitle = _eventObjectRect;
            _isGlobalEventRectTitle = _isGlobalEventRect;
            _methodsDropdownButtonRectTitle = _methodsDropdownButtonRect;
            _globalEventNameTitleRect = _methodsDropdownButtonRect;

            _eventObjectRectTitle.y -= 15;
            _isGlobalEventRectTitle.y -= 15;
            _methodsDropdownButtonRectTitle.y -= 15;
            _globalEventNameTitleRect.y -= 15;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeProperties(property);

            position.y += 20f;
            EditorGUI.BeginProperty(position, label, property);

            InitializePropertyRects(position);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            DrawMainGUI();
            CheckEventTypeChange(property);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void DrawMainGUI()
        {
            if (_eventObjectProperty.objectReferenceValue == null)
            {
                _monobehaviourMethodNameProperty.stringValue = _noneLabel;
                _monobehaviourReferenceProperty.objectReferenceValue = null;
                _isGlobalEventProperty.boolValue = false;
                _globalEventNameProperty.stringValue = string.Empty;
            }

            EditorGUI.LabelField(_eventObjectRectTitle, _eventObjectLabel, GetSlowTextStyle());
            EditorGUI.PropertyField(_eventObjectRect, _eventObjectProperty, GUIContent.none, true);

            EditorGUI.BeginDisabledGroup(_eventObjectProperty.objectReferenceValue == null);

            EditorGUI.LabelField(_isGlobalEventRectTitle, _isGlobalEventLabel, GetSlowTextStyle());
            EditorGUI.PropertyField(_isGlobalEventRect, _isGlobalEventProperty, GUIContent.none, true);
        }

        private void CheckEventTypeChange(SerializedProperty property)
        {
            if (_isGlobalEventProperty.boolValue)
            {
                EditorGUI.LabelField(_globalEventNameTitleRect, _globalEventNameLabel, GetSlowTextStyle());
                EditorGUI.PropertyField(_methodsDropdownButtonRect, _globalEventNameProperty, GUIContent.none, true);
            }
            else
            {
                EditorGUI.LabelField(_methodsDropdownButtonRectTitle, _avaliableMethodsLabel, GetSlowTextStyle());
                _selectedMonobehaviourMethodInfoTitle.text = _monobehaviourMethodNameProperty.stringValue;

                if (EditorGUI.DropdownButton(_methodsDropdownButtonRect, _selectedMonobehaviourMethodInfoTitle, FocusType.Passive))
                {
                    var menu = new GenericMenu();
                    var eventGameObject = _eventObjectProperty.objectReferenceValue as GameObject;
                    var allComponents = eventGameObject.GetComponents<MonoBehaviour>().ToList();

                    if (allComponents.Count == 0)
                    {
                        EditorUtility.DisplayDialog(_warningTitle, _nothingFoundMessage, _okLabel);
                        _selectedMonobehaviourMethodInfoTitle.text = _noneLabel;
                        return;
                    }

                    foreach (var currentComponent in allComponents)
                    {
                        var allComponentMethods = currentComponent.GetType().GetMethods();
                        var filteredMethods = GetFillteredMethods(allComponentMethods);

                        foreach (var currentMethod in filteredMethods)
                        {
                            var componentMethodIndoGUIContent = new GUIContent($"{currentComponent.GetType().Name}/{currentMethod.Name}");
                            var componentMethodInfo = new UnityMonobehaviourMethodInfo(currentComponent, currentMethod);

                            menu.AddItem(componentMethodIndoGUIContent,
                                         _monobehaviourMethodNameProperty.stringValue == componentMethodInfo.MonobehaviourMethodInfo.Name,
                                         (_) =>
                                         {
                                             //Dirty hack - need refactoring
                                             var monobehaviourMethodNamePropertyInternal = property.FindPropertyRelative(_monobehaviourMethodPropertyName);
                                             var monobehaviorReferencePropertyInternal = property.FindPropertyRelative(_monobehaviourReferencePropertyName);

                                             monobehaviourMethodNamePropertyInternal.stringValue = componentMethodInfo.MonobehaviourMethodInfo.Name;
                                             monobehaviorReferencePropertyInternal.objectReferenceValue = componentMethodInfo.MonobehaviourReference;

                                             _selectedMonobehaviourMethodInfoTitle = new GUIContent($"{currentComponent.GetType().Name}/{currentMethod.Name}");
                                             _mainProperty.serializedObject.ApplyModifiedProperties();
                                         },
                                         componentMethodInfo);
                        }

                    }

                    menu.AddItem(new GUIContent(_noneLabel), false, ClearAllMethodInfo, null);
                    menu.ShowAsContext();

                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        private IEnumerable<MethodInfo> GetFillteredMethods(MethodInfo[] allMonobehaviourMethods)
        {
            return allMonobehaviourMethods.Where(x => x.IsPublic &&
                                                !x.ContainsGenericParameters &&
                                                !x.Name.StartsWith("set_") &&
                                                 x.ReturnType == typeof(void) &&
                                                 x.GetParameters().Length == 1 &&
                                                 x.GetParameters()[0].ParameterType == typeof(EventParameter)
                                                );
        }

        private void ClearAllMethodInfo(object obj)
        {
            _selectedMonobehaviourMethodInfoTitle.text = _noneLabel;
            _monobehaviourMethodNameProperty.stringValue = _noneLabel;
            _mainProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 5f;
        }
    }
}

