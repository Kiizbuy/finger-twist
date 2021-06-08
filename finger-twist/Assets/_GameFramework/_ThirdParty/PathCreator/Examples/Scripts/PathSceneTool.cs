using System;
using UnityEngine;

namespace PathCreation.Examples
{
    [ExecuteInEditMode]
    public abstract class PathSceneTool : MonoBehaviour
    {
        public event Action OnDestroyed;
        public PathCreator PathCreator;
        public bool AutoUpdate = true;

        protected VertexPath Path => PathCreator.Path;

        public void TriggerUpdate() 
            => PathUpdated();

        protected virtual void OnDestroy()
            => OnDestroyed?.Invoke();

        protected abstract void PathUpdated();
    }
}
