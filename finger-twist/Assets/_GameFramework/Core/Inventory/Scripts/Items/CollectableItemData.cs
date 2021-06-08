using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.Inventory.Items
{
    [CreateAssetMenu(fileName = "Collectable Item", menuName = "Inventory/Items/Create collectable item")]
    public class CollectableItemData : BaseItemData
    {
        [SerializeField]
        private int _maxCollectionCount = 2;

        public override void PutToInventory(Inventory inventory, int count, Func<BaseItemData, int, ItemState> putNewItem)
        {
            var notFullCollections = GetFullItemStateCollections(inventory);
            var remainingCount = count;

            foreach (var state in notFullCollections)
            {
                var countToPut = Math.Min(remainingCount, _maxCollectionCount - state.ItemsCount);

                state.Data = this;
                state.ItemsCount += countToPut;
                remainingCount -= countToPut;

                if (remainingCount <= 0)
                    return;
            }

            while (remainingCount > 0)
            {
                var countToPut = Math.Min(remainingCount, _maxCollectionCount);
                var state = putNewItem(this, countToPut);

                if (state == null)
                    return;

                state.Data = this;
                remainingCount -= countToPut;
            }
        }

        public override void EjectFromInventory(Inventory inventory, int count, Func<BaseItemData, int, ItemState> ejectItem)
        {
            var fullCollections = GetFullItemStateCollections(inventory);
            var remainingCount = count;

            foreach (var state in fullCollections)
            {
                var countToEject = Math.Min(state.ItemsCount, remainingCount);

                state.ItemsCount -= countToEject;
                remainingCount -= countToEject;

                if (state.ItemsCount <= 0)
                    state.Data = null;

                if (remainingCount <= 0)
                    return;
            }

            while (remainingCount > 0)
            {
                var countToEject = Math.Min(remainingCount, _maxCollectionCount);
                var state = ejectItem(this, countToEject);

                if (state == null)
                    return;

                state.ItemsCount -= countToEject;
                remainingCount -= countToEject;

                if (state.ItemsCount <= 0)
                    state.Data = null;
            }
        }

        private bool ItemDoesNotFitValidationParameters(ItemState itemState)
            => itemState.Data == null || itemState.ItemsCount == 0 || itemState.Data.Title == string.Empty || itemState.Data.GetType() != GetType() || itemState.Data.Title != Title;

        private List<ItemState> GetFullItemStateCollections(Inventory inventory)
        {
            return inventory.Items.Where(state =>
                (state.Data == null || state.ItemsCount == 0) ||
                (state.Data != null && state.Data.GetType() == GetType() && state.Data.Title == Title))
                .ToList();
        }
    }
}
