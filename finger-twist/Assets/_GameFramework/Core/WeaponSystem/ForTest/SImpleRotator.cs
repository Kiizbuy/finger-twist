using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);
    }
}
