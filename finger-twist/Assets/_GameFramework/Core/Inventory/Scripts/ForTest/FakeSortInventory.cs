using UnityEngine;

namespace GameFramework.Inventory
{
    public class FakeSortInventory : MonoBehaviour
    {
        public Inventory Inventory;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
                Inventory?.SortItems();
        }
    }
}
