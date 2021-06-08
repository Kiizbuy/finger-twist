using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GameFramework.Editor.Utils
{
    [InitializeOnLoad]
    public class NewCreatedSceneSetuper
    {
        private static readonly string _debugObjectPath = "Assets/_GameFramework/Prefabs/DebugConsole/DebugObjects.prefab";
        private static readonly string _sceneContextConsolePath = "Assets/_GameFramework/Prefabs/SceneContextPrefab/SceneContext.prefab";


        static NewCreatedSceneSetuper()
        {
            EditorSceneManager.newSceneCreated += SetupNewScene;
        }

        private static void SetupNewScene(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            if (setup == NewSceneSetup.EmptyScene || mode == NewSceneMode.Additive)
                return;

            var mainCamera = Camera.main?.transform;
            var directionalLight = GameObject.Find("Directional Light")?.transform;

            var ui = new GameObject("UI").transform;
            var logic = new GameObject("Logic").transform;
            var debug = new GameObject("Debug").transform;
            var singletons = new GameObject("Singletons").transform;
            var environment = new GameObject("Environment").transform;
            var geo = new GameObject("Geo").transform;
            var staticObjects = new GameObject("Static").transform;
            var dynamicObjects = new GameObject("Dynamic").transform;
            var runtime = new GameObject("Runtime").transform;
            var lighting = new GameObject("Lights").transform;
            var cameras = new GameObject("Cameras").transform;
            var vfx = new GameObject("VFX").transform;
            var audio = new GameObject("Audios").transform;
            var sfx = new GameObject("SFX").transform;
            var music = new GameObject("Music").transform;
            var entites = new GameObject("Entites").transform;

            lighting.SetParent(environment.transform);
            mainCamera?.SetParent(cameras.transform);

            cameras.SetParent(environment.transform);
            directionalLight?.SetParent(lighting.transform);

            geo.SetParent(environment);
            singletons.SetParent(logic);

            staticObjects.SetParent(geo);
            dynamicObjects.SetParent(geo);

            vfx.SetParent(environment);

            audio.SetParent(environment);
            sfx.SetParent(audio);
            music.SetParent(audio);

            staticObjects.gameObject.isStatic = true;
            CreateSceneLogicPrefabs(logic);

            Debug.Log("Setup Scene has been complete");
        }

        private static void CreateSceneLogicPrefabs(Transform logicObject)
        {
            СreateGameObjectFromAssetAtPath(_debugObjectPath, logicObject, (debugConsoleObject) =>
            {
                var debugPlaceholder = logicObject.Find("Debug");
                debugConsoleObject.transform.SetParent(debugPlaceholder);
            });

            СreateGameObjectFromAssetAtPath(_sceneContextConsolePath, logicObject, (sceneContextObject) =>
            {
                sceneContextObject.transform.SetParent(logicObject);
            });

            var singletonsPlaceholder = logicObject.Find("Singletons");
            var uiEventSystem = new GameObject("EventSystem").AddComponent<EventSystem>().transform;

            uiEventSystem.gameObject.AddComponent<StandaloneInputModule>();
            uiEventSystem.SetParent(logicObject);
        }

        public static void СreateGameObjectFromAssetAtPath(string path, Transform logicObject, Action<GameObject> OnObjectExist)
        {
            var loadableObjectAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (loadableObjectAsset != null)
            {
                var loadableObject = PrefabUtility.InstantiatePrefab(loadableObjectAsset) as GameObject;
                OnObjectExist?.Invoke(loadableObject);
            }
            else
            {
                Debug.LogError($"Scene context prefab is not found on path: {_debugObjectPath}");
            }
        }
    }
}
