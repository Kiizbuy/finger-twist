using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Inventory.UI
{
    public class FakeChangeInventoryOwner : MonoBehaviour
    {
        public InventoryUIController InventoryUIController;
        public Inventory Inventory;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                InventoryUIController.ChangeInventoryOwner(Inventory);
        }
    }
}
