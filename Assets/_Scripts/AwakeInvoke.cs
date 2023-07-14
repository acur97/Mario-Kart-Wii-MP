using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class AwakeInvoke : MonoBehaviour
{
    public PostProcessLayer post;
    [Space]
    public UnityEvent start;
    public UnityEvent startEditor;

    private void Start()
    {
        if (Application.isEditor)
        {
            if (Application.isPlaying)
            {
                start.Invoke();
                post.finalBlitToCameraTarget = true;
            }
            else
            {
                startEditor.Invoke();
                post.finalBlitToCameraTarget = false;
            }
        }
        else
        {
            start.Invoke();
            post.finalBlitToCameraTarget = true;
        }
    }
}