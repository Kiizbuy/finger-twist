#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEditorExtensions
{
    public static class EditorExtensions
    {
        public static bool DisplayButton(this Editor editor, string text, bool buttonEnabled)
        {
            GUI.enabled = buttonEnabled;
            bool res = GUILayout.Button(text);
            GUI.enabled = true;
            return res;
        }

        public static UnityEngine.Object GetPrefab(this Editor editor, MonoBehaviour mono)
        {
            var t = PrefabUtility.GetPrefabAssetType(mono.gameObject);

            if (t == PrefabAssetType.Regular || t == PrefabAssetType.Variant)
                return mono.gameObject;
            else
                return PrefabUtility.GetCorrespondingObjectFromSource(mono.gameObject);
        }

        public static void SavePrefab(this Editor editor)
            => SavePrefab(editor, (MonoBehaviour)editor.target);

        public static void SavePrefab(this Editor editor, MonoBehaviour mono)
            => SavePrefab(mono);


        public static void SavePrefab(MonoBehaviour mono)
        {
            var instance = PrefabUtility.GetOutermostPrefabInstanceRoot(mono.gameObject);
            if (instance == null)
            {
                return;
            }
            bool wasActive = instance.activeSelf;
            instance.SetActive(true);
            PrefabUtility.SaveAsPrefabAsset(instance, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instance), out var succes);
            instance.SetActive(wasActive);
        }

        public static GameObject GetPrefabGameObject(this Editor editor)
            => (GameObject)GetPrefab(editor, (MonoBehaviour)editor.target);


        public static T GetObjectValueFromSerializedProperty<T>(this SerializedProperty property) where T : class
        {
            object obj = property.serializedObject.targetObject;
            FieldInfo field = null;

            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                field = type.GetField(path);
                obj = field.GetValue(obj);
            }

            return obj as T;
        }

        public static object GetObjectWhoUseTheProperty(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            FieldInfo field = null;

            foreach (var path in property.propertyPath.Split('.').Where(x => !x.Contains(property.name)))
            {
                var type = obj.GetType();
                field = type.GetField(path);
                obj = field.GetValue(obj);
            }

            return obj;
        }
    }
}

#endif
