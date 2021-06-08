using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Common.ResourcesCore
{
    public interface IResourceFactory
    {
        T Create<T>(string resourceElementPath, Action<T> onCreateDoneCallback = null) where T : Object;
    }

    public class ResourceFactory : IResourceFactory
    {
        public T Create<T>(string resourceElementPath, Action<T> onCreateDoneCallback = null) where T : Object
        {
            var resourceElement = Resources.Load<T>(resourceElementPath);

            if (resourceElement == null)
            {
                Debug.LogError($"{typeof(T).Name} doesn't exist on path: {resourceElementPath}");
                return null;
            }

            onCreateDoneCallback?.Invoke(resourceElement);
            return resourceElement;
        }
    }
}
