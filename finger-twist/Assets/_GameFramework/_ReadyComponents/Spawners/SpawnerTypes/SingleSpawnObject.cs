using System;
using UnityEngine;
using GameFramework.Events;

namespace GameFramework.Components
{
    public class SingleSpawnObject : ISpawner
    {
        [SerializeField] private GameObject _spawnableObject;

        public bool TrySpawn(out GameObject spawnableObject)
        {
            if(_spawnableObject == null)
            {
                spawnableObject = default;
                return false;
            }

            spawnableObject = GameObject.Instantiate(_spawnableObject);
            return true;
        }
    }
}

