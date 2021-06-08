namespace GameFramework.Inventory.Items
{
    public enum EquipmentType
    {
        Head,
        Arms,
        Torso,
        Legs,
        Bag,
        Weapon
    }

    public class EquipableItemData : BaseItemData, IEquipable
    {
        public EquipmentType EquipmentType;

        public void EquipBy(IEntity entity) { }
        public void UnequipBy(IEntity entity) { }
    }
}

