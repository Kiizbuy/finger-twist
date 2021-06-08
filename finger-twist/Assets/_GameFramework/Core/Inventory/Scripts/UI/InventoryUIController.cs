using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Inventory.UI
{
    public class DragItemCellState
    {
        public readonly ItemCell DraggedEntry;
        public readonly RectTransform OriginalParent;

        public DragItemCellState(ItemCell draggedEntry, RectTransform originalParent)
        {
            DraggedEntry = draggedEntry;
            OriginalParent = originalParent;
        }
    }

    public sealed class InventoryUIController : MonoBehaviour
    {
        [SerializeField]
        private Inventory _inventoryOwner;
        [SerializeField]
        private ItemCell _itemCellPrefab;
        [SerializeField]
        private RectTransform[] _itemSlots;
        [SerializeField]
        private Canvas _dragCanvas;

        private ItemCell[] _itemEntriesCells;
        private ItemCell _hoveredItem;

        public DragItemCellState CurrentlyDragged { get; set; }
        public CanvasScaler DragCanvasScaler { get; private set; }
        public Inventory InventoryOwner => _inventoryOwner;
        public Canvas DragCanvas => _dragCanvas;

        public void SetDragCanvas(Canvas value)
            => _dragCanvas = value;

        public void ChangeInventoryOwner(Inventory value)
        {
            if (_inventoryOwner != value)
            {
                _inventoryOwner = value;
                UpdateItemsStateInfo();
            }
        }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            CurrentlyDragged = null;
            DragCanvasScaler = DragCanvas.GetComponentInParent<CanvasScaler>();

            _itemEntriesCells = new ItemCell[_itemSlots.Length];

            for (int i = 0; i < _itemEntriesCells.Length; ++i)
            {
                ClearCell(i);

                _itemEntriesCells[i] = Instantiate(_itemCellPrefab, _itemSlots[i]);
                _itemEntriesCells[i].gameObject.SetActive(false);
                _itemEntriesCells[i].Owner = this;
                _itemEntriesCells[i].InventoryEntryIndex = i;
                _itemEntriesCells[i].UpdateEntry();
            }

            //EquipementUI.Init(this);
        }

        private void ClearCell(int i)
        {
            foreach (Transform t in _itemSlots[i])
                Destroy(t.gameObject);
        }


        public void ObjectHoveredEnter(ItemCell hovered)
        {
            _hoveredItem = hovered;

            ///TODO: Create Tooltip Panel with item description;
        }

        public void HandledDroppedEntry(Vector3 position)
        {
            for (int i = 0; i < _itemSlots.Length; ++i)
            {
                var cellRect = _itemSlots[i];

                if (RectTransformUtility.RectangleContainsScreenPoint(cellRect, position))
                {
                    if (_itemEntriesCells[i] != CurrentlyDragged.DraggedEntry)
                    {
                        _inventoryOwner.SwapItems(CurrentlyDragged.DraggedEntry.InventoryEntryIndex, i);
                        _itemEntriesCells[i].UpdateEntry();

                        CurrentlyDragged.DraggedEntry.UpdateEntry();
                    }
                }
            }
        }

        public void UpdateItemsStateInfo()
        {
            foreach (var itemState in _itemEntriesCells)
                itemState.UpdateEntry();
        }
    }
}