using UnityEngine.SceneManagement;
using UnityEngine;

namespace GameFramework.Components
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        public static T GetInstance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogWarning("Instance not initialized! Have to seek it! WARNING! Bad architecture!");
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        Debug.LogError("Not found instance of SINGLETON object!!!");
                        return null;
                    }
                }

                return instance;
            }
        }

        public static T GetOrCreateInstance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject();
                    instance = go.AddComponent<T>();
                    instance.name = "(singleton) " + typeof(T).ToString();

                    //var singletonParent = GameObject.Find("Singletons").transform;

                    //if (singletonParent != null)
                    //    go.transform.SetParent(singletonParent);

                }

                return instance;
            }
        }

        public static bool HasInstance => instance != null;

        protected virtual bool OrderDontDestroyOnLoad => false;

        private void Awake()
        {
            if (instance != null)
            {
                if (instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                instance = (T)(object)this;
            }

            if (OrderDontDestroyOnLoad)
            {
                if (gameObject.transform.parent != null)
                {
                    gameObject.transform.SetParent(null);
                }
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            OnAwake();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) { }
    }
}
