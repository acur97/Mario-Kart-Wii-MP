using UnityEngine;

[ExecuteInEditMode]
public class Sharpen : MonoBehaviour
{
    [Range(0, 1)]
    [Tooltip("Sharpness")]
    public float Sharpness = 0.1f;

    public Material material;

    private readonly int CentralFactor = Shader.PropertyToID("_CentralFactor");
    private readonly int SideFactor = Shader.PropertyToID("_SideFactor");

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat(CentralFactor, 1.0f + (3.2f * Sharpness));
        material.SetFloat(SideFactor, 0.8f * Sharpness);
        Graphics.Blit(source, destination, material, 0);
    }
}
