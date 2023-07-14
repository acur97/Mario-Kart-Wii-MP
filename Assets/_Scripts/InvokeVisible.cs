using UnityEngine;
using UnityEngine.Events;

public class InvokeVisible : MonoBehaviour
{
    public UnityEvent OnVisible;
    public UnityEvent OnInvisible;

    private void OnBecameVisible()
    {
        OnVisible.Invoke();
    }

    private void OnBecameInvisible()
    {
        OnInvisible.Invoke();
    }
}