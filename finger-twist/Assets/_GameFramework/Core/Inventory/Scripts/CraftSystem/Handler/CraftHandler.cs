using System.Collections.Generic;
using UnityEngine;
using GameFramework.Inventory;
using GameFramework.Inventory.Items;
using UnityEngine.Events;

namespace GameFramework.Inventory
{
    [DisallowMultipleComponent]
    public sealed class CraftHandler : MonoBehaviour
    {
        public enum CraftingType
        {
            Consecutive,
            Simultaneous
        }

        public UnityEvent OnCrafted;

        [SerializeField] private Inventory _inventoryOwner;
        [SerializeField] private CraftingType _itemCraftType;

        private readonly List<CraftingQueueItemObject> _craftItemQueues = new List<CraftingQueueItemObject>();

        public void ChangeInventoryOwner(Inventory owner)
            => _inventoryOwner = owner;

        private void Update()
        {
            CalculateCraftingQueue();
        }

        private void CalculateCraftingQueue()
        {
            if (_inventoryOwner == null)
                return;

            var queueCount = _craftItemQueues.Count;

            if (_itemCraftType == CraftingType.Consecutive && queueCount > 0)
                Work(0);
            else if (_itemCraftType == CraftingType.Simultaneous && queueCount > 0)
                for (int i = queueCount - 1; i >= 0; i--)
                    Work(i);
        }

        private void Work(int index)
        {
            var queue = _craftItemQueues[index];

            queue.Timer = Mathf.Clamp(queue.Timer - Time.deltaTime, 0, queue.Timer + Time.deltaTime);
            _craftItemQueues[index] = queue;

            if (Mathf.Approximately(queue.Timer, 0))
            {
                _inventoryOwner.AddItem(queue.Item, queue.Amount);
                _craftItemQueues.RemoveAt(index);

                OnCrafted.Invoke();
            }
        }


        public bool TryCraft(CraftRecipeData recipe, int count)
        {
            if (recipe == null || count <= 0)
            {
                Debug.LogError("Recipe argument null or count argument <= 0");
                return false;
            }

            foreach (var ingredient in recipe.Ingredients)
                if (_inventoryOwner.GetItemStateViaName(ingredient.Item.Title).ItemsCount < ingredient.Count * count)
                    return false;

            foreach (var ingredient in recipe.Ingredients)
                _inventoryOwner.RemoveItem(ingredient.Item, ingredient.Count * count);

            _craftItemQueues.Add(new CraftingQueueItemObject(recipe.Product.Item, recipe.Product.Count, recipe.CraftingTime));

            return true;
        }
    }
}

