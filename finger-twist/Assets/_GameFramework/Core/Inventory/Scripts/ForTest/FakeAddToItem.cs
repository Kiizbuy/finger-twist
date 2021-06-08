using GameFramework.Inventory;
using GameFramework.Inventory.Items;
using UnityEngine;

public class FakeAddToItem : MonoBehaviour
{
    [OneLine.OneLineWithHeader]
    public ItemState BaseItemState;
    public GameFramework.Inventory.Inventory Inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            Inventory.AddItem(BaseItemState.Data, BaseItemState.ItemsCount);

    }
}
