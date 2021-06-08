using UnityEngine;

public class MeleeWeaponTest : MonoBehaviour
{
    public Animator Animator;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Attack");
        }
    }
}
