using GameFramework.Events;
using GameFramework.Strategy;
using System;
using UnityEngine;

namespace GameFramework.Components
{
    public class ObjectSpawner : MonoBehaviour
    {
        public event Action<EventParameter> OnSpawned;

        [EventName(nameof(OnSpawned))]
        public EventToMethodSubscribeСontainer OnSpawnedSubscriber = new EventToMethodSubscribeСontainer();
        [Space(5f)]
        [MethodName(nameof(SpawnTrigger))]
        public MethodToEventSubscribeContainer SpawnTriggerSubscriber = new MethodToEventSubscribeContainer();

        [Header("Strategies")]
        [SerializeReference, StrategyContainer]
        public ISpawner SpawnerType;
        [SerializeReference, StrategyContainer]
        public IObjectPlacer ObjectPlaceType;

        private void Start()
        {
            if (SpawnerType == null)
            {
                Debug.LogError("Spawner type is null, please, assign him", this);
                return;
            }
            if (ObjectPlaceType == null)
            {
                Debug.LogError("Object Place Type is null, please, assign him", this);
                return;
            }

            EventSubscriber.Subscribe(this);
        }

        private void OnDrawGizmosSelected()
        {
            var selected = ObjectPlaceType as IStrategyDrawGizmosSelected;

            if (ObjectPlaceType != null)
                selected?.DrawGizmosSelected();
        }

        public void SpawnTrigger(EventParameter parameter)
        {
            if (SpawnerType.TrySpawn(out var spawnedGameObject))
            {
                ObjectPlaceType.Place(spawnedGameObject);
                OnSpawned?.Invoke(new EventParameter_GameObject(spawnedGameObject));
            }
        }
    }

}
