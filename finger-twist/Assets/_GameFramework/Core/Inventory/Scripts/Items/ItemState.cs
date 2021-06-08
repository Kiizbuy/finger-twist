using System;
using NaughtyAttributes;

namespace GameFramework.Inventory.Items
{
    [Serializable]
    public class ItemState
    {
        public BaseItemData Data;
        public int ItemsCount;
    }
}
