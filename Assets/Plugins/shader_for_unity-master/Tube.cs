using UnityEngine;

[ExecuteInEditMode]
public class Tube : MonoBehaviour
{
    public Material _material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material, 0);
    }
}