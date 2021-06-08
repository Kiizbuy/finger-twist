using GameFramework.Strategy;
using UnityEngine;

namespace GameFramework.Components
{
    public interface ISpawner : IStrategyContainer
    {
        bool TrySpawn(out GameObject spawnableObject);
    }
}

