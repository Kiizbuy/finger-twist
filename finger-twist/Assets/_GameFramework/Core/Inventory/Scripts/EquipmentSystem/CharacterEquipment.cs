using System;
using UnityEngine;
using GameFramework.Inventory.Items;

namespace GameFramework.Inventory.Equipment
{
    public class CharacterEquipment : MonoBehaviour
    {
        public event Action<EquipableItemData> OnEquip;
        public event Action<EquipableItemData> OnUnequip;

        [SerializeField] private EquipableItemData _headItem;
        [SerializeField] private EquipableItemData _armsItem;
        [SerializeField] private EquipableItemData _torsoItem;
        [SerializeField] private EquipableItemData _legsItem;
        [SerializeField] private EquipableItemData _bagItem;
        [SerializeField] private EquipableItemData _firstWeaponItem;
        //[SerializeField] private EquipableItemData _secondWeaponItem;

        private IEntity _character;

        public EquipableItemData GetEquipableItem(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Head:
                    return _headItem;
                case EquipmentType.Arms:
                    return _armsItem;
                case EquipmentType.Torso:
                    return _torsoItem;
                case EquipmentType.Legs:
                    return _legsItem;
                case EquipmentType.Bag:
                    return _bagItem;
                case EquipmentType.Weapon:
                    ///TODO: Create change first-secon weapon state
                    return _firstWeaponItem;

                default:
                    {
                        Debug.LogError("Unknown item type", this);
                        return null;
                    }
            }
        }

        public void ChangeCharacter(IEntity newCharacter)
        {
            _character = newCharacter;
        }

        public void Equip(EquipableItemData equipableItem)
        {

        }

        public void Unequip(EquipableItemData equipableItem)
        {

        }
    }

}

