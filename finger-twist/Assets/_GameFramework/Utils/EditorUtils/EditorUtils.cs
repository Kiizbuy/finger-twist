#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Utils.Editor
{
    public static class EditorUtils
    {
        public static T[] GetAllInstances<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var allInstancesOfType = new T[guids.Length];

            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                allInstancesOfType[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return allInstancesOfType;

        }

        public static string GetMonoScriptPathFor(Type type)
        {
            var asset = string.Empty;
            var guids = AssetDatabase.FindAssets($"{type.Name} t:script");

            if (guids.Length > 1)
            {
                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var filename = Path.GetFileNameWithoutExtension(assetPath);
                    if (filename != type.Name)
                        continue;

                    asset = guid;
                    break;
                }
            }
            else if (guids.Length == 1)
            {
                asset = guids[0];
            }
            else
            {
                Debug.LogError($"Unable to locate {type.Name}");
                return null;
            }

            return AssetDatabase.GUIDToAssetPath(asset);
        }
    }
}

#endif
