using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Inventory.Items;

public class FakeRemoveItemFromInventory : MonoBehaviour
{
    [OneLine.OneLineWithHeader]
    public ItemState BaseItemState;
    public GameFramework.Inventory.Inventory Inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            Inventory.RemoveItem(BaseItemState.Data, BaseItemState.ItemsCount);
    }
}
