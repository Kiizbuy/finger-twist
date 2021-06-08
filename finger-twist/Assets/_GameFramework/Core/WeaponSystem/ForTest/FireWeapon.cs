using GameFramework.WeaponSystem;
using UnityEngine;

public class FireWeapon : MonoBehaviour
{
    public GunWeapon Weapon;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            Weapon?.Attack();
        else
            Weapon?.StopAttack();

        if (Input.GetKeyDown(KeyCode.R))
            Weapon?.ReloadWeapon();
    }
}
