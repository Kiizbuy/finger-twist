using UnityEngine;
using UnityEngine.Events;

public class PoolObject : MonoBehaviour
{
    public UnityEvent OnReUse;
    public UnityEvent OnDestroyObject;

    public void ReUseObject()
        => OnReUse?.Invoke();
    public void DestroyObject()
        => OnDestroyObject?.Invoke();
}
