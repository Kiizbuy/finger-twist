using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    [ExecuteInEditMode]
    public class PathFollower : MonoBehaviour
    {
        public PathCreator PathCreator;
        public EndOfPathInstruction EndOfPathInstruction;
        public float Time = 0f;
        public float Speed = 5;
        private float distanceTravelled;

        private void Start()
        {
            if (PathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                PathCreator.OnPathUpdated += OnPathChanged;
            }
        }

        private void Update()
        {
            if (PathCreator != null)
            {
                transform.position = PathCreator.Path.GetPointAtTime(Time, EndOfPathInstruction);
                //distanceTravelled += Speed * Time.deltaTime;
                //transform.position = PathCreator.Path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction);
                //transform.rotation = PathCreator.Path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction);
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        private void OnPathChanged() 
             => distanceTravelled = PathCreator.Path.GetClosestDistanceAlongPath(transform.position);
    }
}