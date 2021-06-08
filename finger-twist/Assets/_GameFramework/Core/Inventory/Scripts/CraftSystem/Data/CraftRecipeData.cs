using System.Collections.Generic;
using UnityEngine;
using OneLine;

namespace GameFramework.Inventory.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Craft/Create craft recipe data")]
    public class CraftRecipeData : ScriptableObject
    {
        public string Name;
        public float CraftingTime;
        [OneLineWithHeader]
        public List<CraftingIngredientState> Ingredients = new List<CraftingIngredientState>();
        public CraftingIngredientState Product;
    }
}

