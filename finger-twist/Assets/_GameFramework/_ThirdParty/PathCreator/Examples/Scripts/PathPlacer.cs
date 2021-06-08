using PathCreation;
using UnityEngine;

namespace PathCreation.Examples
{
    [ExecuteInEditMode]
    public class PathPlacer : PathSceneTool
    {
        public GameObject prefab;
        public GameObject holder;
        public float spacing = 3;

        private readonly float minSpacing = .1f;

        private void Generate()
        {
            if (PathCreator != null && prefab != null && holder != null)
            {
                DestroyObjects();

                var path = PathCreator.Path;
                var dst = 0f;

                spacing = Mathf.Max(minSpacing, spacing);

                while (dst < path.length)
                {
                    var point = path.GetPointAtDistance(dst);
                    var rot = path.GetRotationAtDistance(dst);
                    Instantiate(prefab, point, rot, holder.transform);
                    dst += spacing;
                }
            }
        }

        private void DestroyObjects()
        {
            var numChildren = holder.transform.childCount;

            for (int i = numChildren - 1; i >= 0; i--)
                DestroyImmediate(holder.transform.GetChild(i).gameObject, false);
        }

        protected override void PathUpdated()
        {
            if (PathCreator != null)
                Generate();
        }
    }
}