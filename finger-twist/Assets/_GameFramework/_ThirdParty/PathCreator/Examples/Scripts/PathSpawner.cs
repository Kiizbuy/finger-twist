using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace PathCreation.Examples
{
    public class PathSpawner : MonoBehaviour
    {
        public PathCreator PathObject;
        public PathFollower FollowerPrefab;
        public Transform[] SpawnPoints;

        private void Start()
        {
            foreach (Transform t in SpawnPoints)
            {
                var path = Instantiate(PathObject, t.position, t.rotation);
                var follower = Instantiate(FollowerPrefab);
                follower.PathCreator = path;
                path.transform.parent = t;

            }
        }
    }

}