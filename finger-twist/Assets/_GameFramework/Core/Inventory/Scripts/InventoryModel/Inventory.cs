using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using GameFramework.Inventory.Items;
using GameFramework.Extension;

namespace GameFramework.Inventory
{
    public sealed class Inventory : MonoBehaviour
    {
        public UnityEvent OnItemsStateChanged;
        public UnityEvent OnInventoryFull;

        [SerializeField, OneLine.OneLineWithHeader]
        private List<ItemState> _items = new List<ItemState>();

        public IReadOnlyCollection<ItemState> Items => _items;

        public ItemState GetItemStateViaIndex(int itemIndex)
        {
            if(itemIndex > _items.Count - 1 || itemIndex < 0)
            {
                Debug.LogError($"Index {itemIndex} is out of range", this);
                return null;
            }

            return _items[itemIndex];
        }

        public ItemState GetItemStateViaName(string itemName)
            => _items.FirstOrDefault(state => state.Data.Title == itemName);

        public void AddItem(BaseItemData item, int count, UnityAction<BaseItemData> OnAddingItemHasComplete = null)
        {
            item.PutToInventory(this, count, (addableItem, countToPut) => PutNewItem(item, countToPut));
            OnItemsStateChanged?.Invoke();
        }

        public void RemoveItem(BaseItemData item, int count, UnityAction<BaseItemData> OnRemovingItemHasComplete = null)
        {
            item.EjectFromInventory(this, count, (removableItem, countToEject) => RemoveOldItem(item, countToEject));
            OnItemsStateChanged?.Invoke();
        }

        private ItemState FindNotEmptyItemState(BaseItemData item, int count)
            => _items.Where(state => state.Data != null &&
                                     state.Data.GetType() == item.GetType() && 
                                     state.Data.Title == item.Title &&
                                     state.ItemsCount >= count)
                                    .Reverse()
                                    .FirstOrDefault();

        private ItemState FindEmptyItemState()
            => _items.Where(state => state.ItemsCount == 0 || state.Data == null).FirstOrDefault();

        private ItemState PutNewItem(BaseItemData item, int count)
        {
            var state = FindEmptyItemState();

            if (state == null)
            {
                Debug.Log("Inventory is full");
                OnInventoryFull?.Invoke();
                return null;
            }

            state.Data = item;
            state.ItemsCount = count;

            return state;
        }

        private ItemState RemoveOldItem(BaseItemData item, int count)
        {
            var state = FindNotEmptyItemState(item, count);

            if(state == null)
            {
                Debug.Log($"Not found item {item.Title}");
                return null;
            }

            return state;
        }

        public void SwapItems(int startIndex, int endIndex)
        {
            _items.Swap(startIndex, endIndex);
            OnItemsStateChanged?.Invoke();
        }

        public void SortItems()
        {
            _items = _items.OrderBy(x => -x.ItemsCount).ToList();
            OnItemsStateChanged?.Invoke();
        }
    }
}
