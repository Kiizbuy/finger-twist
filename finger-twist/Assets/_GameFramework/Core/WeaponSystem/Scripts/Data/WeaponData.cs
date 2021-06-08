using NaughtyAttributes;
using UnityEngine;

namespace GameFramework.WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapon/Create weapon data", fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField, MinMaxSlider(0f, 350f)] protected Vector2 _minMaxDamage;

        public int Damage => (int)Random.Range(_minMaxDamage.x, _minMaxDamage.y);
    }
}
