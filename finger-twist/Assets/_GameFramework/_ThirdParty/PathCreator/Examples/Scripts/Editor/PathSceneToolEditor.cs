using UnityEngine;
using UnityEditor;
using PathCreation;

namespace PathCreation.Examples
{
    [CustomEditor(typeof(PathSceneTool), true)]
    public class PathSceneToolEditor : Editor
    {
        protected PathSceneTool pathTool;
        private bool isSubscribed;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                DrawDefaultInspector();

                if (check.changed)
                {
                    if (!isSubscribed)
                    {
                        GetTryFindPathCreator();
                        Subscribe();
                    }

                    if (pathTool.AutoUpdate)
                    {
                        TriggerUpdate();

                    }
                }
            }

            if (GUILayout.Button("Manual Update"))
            {
                if (GetTryFindPathCreator())
                {
                    TriggerUpdate();
                    SceneView.RepaintAll();
                }
            }

        }

        private void TriggerUpdate()
        {
            if (pathTool.PathCreator != null)
                pathTool.TriggerUpdate();
        }


        protected virtual void OnPathModified()
        {
            if (pathTool.AutoUpdate)
                TriggerUpdate();
        }

        protected virtual void OnEnable()
        {
            pathTool = (PathSceneTool)target;
            pathTool.OnDestroyed += OnToolDestroyed;

            if (GetTryFindPathCreator())
            {
                Subscribe();
                TriggerUpdate();
            }
        }

        private void OnToolDestroyed()
        {
            if (pathTool != null)
                pathTool.PathCreator.OnPathUpdated -= OnPathModified;
        }


        protected virtual void Subscribe()
        {
            if (pathTool.PathCreator != null)
            {
                isSubscribed = true;
                pathTool.PathCreator.OnPathUpdated -= OnPathModified;
                pathTool.PathCreator.OnPathUpdated += OnPathModified;
            }
        }

        private bool GetTryFindPathCreator()
        {
            // Try find a path creator in the scene, if one is not already assigned
            if (pathTool.PathCreator == null)
            {
                if (pathTool.GetComponent<PathCreator>() != null)
                    pathTool.PathCreator = pathTool.GetComponent<PathCreator>();
                else if (FindObjectOfType<PathCreator>())
                    pathTool.PathCreator = FindObjectOfType<PathCreator>();
            }
            return pathTool.PathCreator != null;
        }
    }
}