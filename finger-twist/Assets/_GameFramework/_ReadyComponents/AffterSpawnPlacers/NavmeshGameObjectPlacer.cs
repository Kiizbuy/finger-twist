using UnityEngine;
using UnityEngine.AI;
using GameFramework.Events;
using NaughtyAttributes;
using GameFramework.Extension;

namespace GameFramework.Components.ObjectPlacers
{
    public interface IGameObjectPlacer
    {
        void Place(EventParameter param);
    }

    public class RandomNavmeshGameObjectPlacer : MonoBehaviour, IGameObjectPlacer
    {
        [MethodName(nameof(Place))]
        public MethodToEventSubscribeContainer PlaceSubscriber = new MethodToEventSubscribeContainer();

        [SerializeField, Required] private Transform _placePoint;
        [SerializeField, Range(0.01f, 100f)] private float _placeRange = 1f;

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }

        public void Place(EventParameter param)
        {
            if (param == null)
            {
                Debug.LogError("EventParam is null", gameObject);
                return;
            }

            if (TryGetRandomNavmeshPointPoint(_placePoint.position, _placeRange, out var placePoint))
                param.GetGameObject().transform.position = placePoint;
        }

        private bool TryGetRandomNavmeshPointPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                var randomPoint = center.GetRandomCirclePoint(range);
                if (NavMesh.SamplePosition(randomPoint, out var hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}

