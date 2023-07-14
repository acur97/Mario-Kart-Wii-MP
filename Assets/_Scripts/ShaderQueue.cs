using UnityEngine;

[ExecuteInEditMode]
public class ShaderQueue : MonoBehaviour
{
    public int Queue = 3010;

    private void Awake()
    {
        GetComponent<MeshRenderer>().sharedMaterial.renderQueue = Queue;
    }
}