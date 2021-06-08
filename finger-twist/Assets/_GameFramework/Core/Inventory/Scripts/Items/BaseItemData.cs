using System;
using UnityEngine;

namespace GameFramework.Inventory.Items
{
    public interface IEntity
    {

    }

    public interface IUsableItem
    {
        void Use(IEntity entity, Inventory inventory);
    }

    public interface IEquipable
    {
        void EquipBy(IEntity entity);
        void UnequipBy(IEntity entity);
    }

    public class BaseItemData : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _title;
        [SerializeField] private int _price;
        [SerializeField] private string _description;

        public Sprite Icon => _icon;
        public string Title => _title;
        public int Price => _price;
        public virtual string GetDescription() => _description;

        public virtual void PutToInventory(Inventory inventory, int count, Func<BaseItemData, int, ItemState> putNewItem)
        {
            for (int i = 0; i < count; i++)
            {
                var state = putNewItem(this, 1);

                if (state == null)
                    return;
            }
        }

        public virtual void EjectFromInventory(Inventory inventory, int count, Func<BaseItemData, int, ItemState> ejectItem)
        {
            for (int i = 0; i < count; i++)
            {
                var state = ejectItem(this, 1);

                if (state == null)
                    return;

                state.Data = null;
                state.ItemsCount = 0;
            }
        }
    }
}

