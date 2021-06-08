using System;
using GameFramework.Strategy;
using ModestTree.Util;
using UnityEngine;

namespace Game
{
    public interface IMoveButtonCondition : IStrategyContainer
    {
        bool CanMove();
    }

    public interface IPlaceButtonCondition : IStrategyContainer
    {
        bool CanPlace(Vector3 buttonPoint, Action<Vector3> onCanPlace);
    }

    public sealed class DistanceToPlacePointCondition : IPlaceButtonCondition
    {
        [SerializeField] private Transform _placePoint;
        
        public bool CanPlace(Vector3 buttonPoint, Action<Vector3> onCanPlace)
        {
            if (Vector3.Distance(_placePoint.position, buttonPoint) <= 1f)
            {
                onCanPlace?.Invoke(_placePoint.position);
                return true;
            }

            return false;
        }
    }
}
