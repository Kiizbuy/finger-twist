using UnityEngine;

namespace GameFramework.InteractableSystem
{
    public class GameObjectSwitcher : MonoBehaviour, IInteractableObject
    {
        [SerializeField] private GameObject _switchableGameObject;

        public void Interact()
        {
            _switchableGameObject.SetActive(!_switchableGameObject.activeSelf);
        }

        public bool InteractRequirements()
        {
            return true;
        }
    }
}
