using GameFramework.Strategy;
using UnityEngine;

namespace GameFramework.Components
{
    public interface IObjectPlacer : IStrategyContainer
    {
        void Place(GameObject placeableObject);
    }
}

