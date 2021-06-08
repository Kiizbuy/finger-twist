using System;

namespace GameFramework.Inventory.Items
{
    [Serializable]
    public class CraftingQueueItemObject
    {
        public readonly BaseItemData Item;
        public readonly int Amount;
        public float Timer;

        public CraftingQueueItemObject(BaseItemData item, int amount, float timer)
        {
            Item = item;
            Amount = amount;
            Timer = timer;
        }
    }
}
