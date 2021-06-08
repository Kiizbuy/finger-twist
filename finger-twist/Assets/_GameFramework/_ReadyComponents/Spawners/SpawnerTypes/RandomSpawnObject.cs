using GameFramework.Events;
using GameFramework.Extension;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Components
{
    public class RandomSpawnObject : ISpawner
    {
        [SerializeField] private List<GameObject> _randomObjects = new List<GameObject>();

        public bool TrySpawn(out GameObject spawnableObject)
        {
            if(_randomObjects.Count == 0)
            {
                spawnableObject = default;
                return false;
            }

            spawnableObject = GameObject.Instantiate(_randomObjects.GetRandomElement());
            return true;
        }
    }
}

