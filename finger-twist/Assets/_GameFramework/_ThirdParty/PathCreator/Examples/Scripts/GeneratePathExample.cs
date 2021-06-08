using UnityEngine;

namespace PathCreation.Examples
{
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]
    public class GeneratePathExample : MonoBehaviour
    {
        public bool ClosedLoop = true;
        public Transform[] Waypoints;

        private void Start()
        {
            if (Waypoints.Length > 0)
            {
                // Create a new bezier path from the waypoints.
                var bezierPath = new BezierPath(Waypoints, ClosedLoop, PathSpace.xyz);
                GetComponent<PathCreator>().BezierPath = bezierPath;
            }
        }
    }
}