using UnityEngine;
using System.Collections.Generic;
using GameFramework.Extension;

namespace GameFramework.Components.Pooling
{
    public class PoolContainer : SingletonBehaviour<PoolContainer>
    {
        private readonly Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

        public void CreatePool(GameObject prefab, int poolSize)
        {
            var poolKey = prefab.GetInstanceID();

            if (!poolDictionary.ContainsKey(poolKey))
            {
                poolDictionary.Add(poolKey, new Queue<ObjectInstance>());

                var poolHolder = new GameObject(prefab.name + " pool");
                poolHolder.transform.parent = transform;

                for (int i = 0; i < poolSize; i++)
                {
                    var newObject = new ObjectInstance(Instantiate(prefab));
                    poolDictionary[poolKey].Enqueue(newObject);
                    newObject.SetParent(poolHolder.transform);
                }
            }
        }

        public void ReuseObject(GameObject prefab) => ReuseGameObject(prefab, Vector3.zero, Quaternion.identity);
        public void ReuseObject(GameObject prefab, Vector3 position) => ReuseGameObject(prefab, position, Quaternion.identity);

        public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var poolKey = prefab.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                var objectToReuse = poolDictionary[poolKey].Dequeue();
                poolDictionary[poolKey].Enqueue(objectToReuse);

                objectToReuse.Reuse(position, rotation);
            }
        }

        public GameObject ReuseGameObject(GameObject prefab) => ReuseGameObject(prefab, Vector3.zero, Quaternion.identity);
        public GameObject ReuseGameObject(GameObject prefab, Vector3 position) => ReuseGameObject(prefab, position, Quaternion.identity);

        public GameObject ReuseGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var poolKey = prefab.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                var objectToReuse = poolDictionary[poolKey].Dequeue();
                poolDictionary[poolKey].Enqueue(objectToReuse);

                return objectToReuse.ReuseGameObject(position, rotation);
            }

            return null;
        }

        public Transform ReuseTransform(GameObject prefab)
            => ReuseTransform(prefab, Vector3.zero, Quaternion.identity);
        public Transform ReuseTransform(GameObject prefab, Vector3 position)
            => ReuseTransform(prefab, position, Quaternion.identity);
        public Transform ReuseTransform(GameObject prefab, Vector3 position, Quaternion rotation)
            => ReuseGameObject(prefab).transform;

        public void DestroyPoolObject(GameObject prefab)
        {
            var poolKey = prefab.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                var objectToReuse = poolDictionary[poolKey].Dequeue();
                poolDictionary[poolKey].Enqueue(objectToReuse);

                objectToReuse.DestroyObject();
            }
        }

        public class ObjectInstance
        {
            private GameObject gameObject;
            private Transform transform;
            private PoolObject poolObjectScript;
            private bool hasPoolObjectComponent;

            public ObjectInstance(GameObject objectInstance)
            {
                gameObject = objectInstance;
                transform = gameObject.transform;
                gameObject.SetActive(false);

                if (gameObject.GetOrAddComponent<PoolObject>())
                    hasPoolObjectComponent = true;
            }

            public void Reuse()
                => Reuse(Vector3.zero, Quaternion.identity);
            public void Reuse(Vector3 position)
                => Reuse(position, Quaternion.identity);

            public void Reuse(Vector3 position, Quaternion rotation)
            {
                gameObject.SetActive(true);
                transform.position = position;
                transform.rotation = rotation;

                if (hasPoolObjectComponent)
                {
                    poolObjectScript.ReUseObject();
                }
            }

            public GameObject ReuseGameObject()
                => ReuseGameObject(Vector3.zero, Quaternion.identity);
            public GameObject ReuseGameObject(Vector3 position)
                => ReuseGameObject(position, Quaternion.identity);

            public GameObject ReuseGameObject(Vector3 position, Quaternion rotation)
            {
                gameObject.SetActive(true);
                transform.position = position;
                transform.rotation = rotation;

                if (hasPoolObjectComponent)
                    poolObjectScript.ReUseObject();

                return gameObject;
            }

            public Transform ReuseTransform()
                => ReuseTransform(Vector3.zero, Quaternion.identity);
            public Transform ReuseTransform(Vector3 position)
                => ReuseTransform(position, Quaternion.identity);

            public Transform ReuseTransform(Vector3 position, Quaternion rotation)
                => ReuseGameObject(position, rotation).transform;

            public void DestroyObject()
            {
                gameObject.SetActive(false);

                if (hasPoolObjectComponent)
                    poolObjectScript.DestroyObject();
            }
            public void SetParent(Transform parent) => transform.SetParent(parent);
        }
    }
}

