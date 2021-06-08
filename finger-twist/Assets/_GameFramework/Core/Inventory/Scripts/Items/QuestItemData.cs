using System;
using UnityEngine;

namespace GameFramework.Inventory.Items
{
    [CreateAssetMenu(fileName = "Quest Item", menuName = "Inventory/Items/Create quest item")]
    public class QuestItemData : BaseItemData
    {
        public override void EjectFromInventory(Inventory inventory, int count, Func<BaseItemData, int, ItemState> ejectItem)
        {
        }
    }
}
