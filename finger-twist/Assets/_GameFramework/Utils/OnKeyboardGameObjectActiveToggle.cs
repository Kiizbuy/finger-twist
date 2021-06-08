using UnityEngine;

namespace GameFramework.Components
{
    public class OnKeyboardGameObjectActiveToggle : MonoBehaviour
    {
        [SerializeField] private GameObject[] _activableGameObjects;
        [SerializeField] private bool _defaultActiveState = false;
        [SerializeField] private KeyCode _keyCodeToggle = KeyCode.F1;

        private void Start()
        {
            foreach (var toggleGameObject in _activableGameObjects)
                toggleGameObject.SetActive(_defaultActiveState);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(_keyCodeToggle))
                return;

            foreach (var toggleGameObject in _activableGameObjects)
                toggleGameObject.SetActive(!toggleGameObject.activeSelf);
        }
    }
}
