using UnityEngine;
using UnityEngine.UI;
using GameFramework.Inventory.Items;
using UnityEngine.EventSystems;

namespace GameFramework.Inventory.UI
{
    public class ItemCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image ItemIcon;
        public Text ItemCount;
        public int ItemIndex;

        public int InventoryEntryIndex { get; set; } = -1;
        public InventoryUIController Owner { get; set; }
        public int Index { get; set; }
        public ItemState EquipmentItemState { get; private set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount % 2 == 0)
            {
                Debug.Log("Double Clicked");
                //if (InventoryEntry != -1)
                //{
                //    var itemState = Owner.Inventory.GetItemStateViaIndex(InventoryEntry);

                //    if (itemState != null)
                //        Owner.ObjectDoubleClicked(itemState);
                //}
                //else
                //{
                //    Owner.EquipmentDoubleClicked(EquipmentItemState);
                //}
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (EquipmentItemState != null)
                return;

            Owner.CurrentlyDragged = new DragItemCellState(this, (RectTransform)transform.parent);
            transform.SetParent(Owner.DragCanvas.transform, true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.localPosition = transform.localPosition + UnscaleEventDeltaPosition(eventData.delta);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Owner.HandledDroppedEntry(eventData.position);
            transform.SetParent(Owner.CurrentlyDragged.OriginalParent, true);
            transform.position = Owner.CurrentlyDragged.OriginalParent.position;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void UpdateEntry()
        {
            var entry = Owner.InventoryOwner.GetItemStateViaIndex(InventoryEntryIndex);
            var isEnabled = entry != null && entry.Data != null;

            gameObject.SetActive(isEnabled);

            if (isEnabled)
            {
                ItemIcon.sprite = entry.Data.Icon;

                if (entry.ItemsCount > 0)
                {
                    ItemCount.gameObject.SetActive(true);
                    ItemCount.text = entry.ItemsCount > 1 ? entry.ItemsCount.ToString() : string.Empty;
                }
                else
                {
                    ItemCount.gameObject.SetActive(false);
                }
            }
        }

        private Vector3 UnscaleEventDeltaPosition(Vector3 point)
        {
            var referenceResolution = Owner.DragCanvasScaler.referenceResolution;
            var currentResolution = new Vector2(Screen.width, Screen.height);

            var widthRatio = currentResolution.x / referenceResolution.x;
            var heightRatio = currentResolution.y / referenceResolution.y;
            var ratio = Mathf.Lerp(widthRatio, heightRatio, Owner.DragCanvasScaler.matchWidthOrHeight);

            return point / ratio;
        }
    }
}

