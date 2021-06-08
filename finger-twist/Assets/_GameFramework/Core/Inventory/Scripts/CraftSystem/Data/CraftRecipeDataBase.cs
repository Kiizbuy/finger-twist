using System.Collections.Generic;
using UnityEngine;
using OneLine;

namespace GameFramework.Inventory.Items
{
    [System.Serializable]
    public class CraftingIngredientState
    {
        public BaseItemData Item;
        public int Count;
    }

    [CreateAssetMenu(menuName = "Inventory/Items/Craft/Create craft data base")]
    public class CraftRecipeDataBase : ScriptableObject
    {
        public CraftingIngredientState Product => _product;
        public string Name => name;
        public float CraftingTime => _craftingTime;
        public IEnumerable<CraftingIngredientState> Ingredients => _ingredients;

        [SerializeField] private string _name;
        [SerializeField] private CraftingIngredientState _product;
        [SerializeField] private float _craftingTime = 1f;
        [SerializeField, OneLineWithHeader] private List<CraftingIngredientState> _ingredients = new List<CraftingIngredientState>();
    }

}
