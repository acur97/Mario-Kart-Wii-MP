using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TextureOffset : MonoBehaviour
{
    public Vector2 Offset1 = Vector2.zero;
    private Vector2 _offset1 = Vector2.zero;
    public Vector2 Offset2 = Vector2.zero;
    private Vector2 _offset2 = Vector2.zero;
    [Space]
    public bool medioRendimientoEditor = true;

    private Material mat;
    private int uv1 = Shader.PropertyToID("_MainTex");
    private int uv2 = Shader.PropertyToID("_DetailAlbedoMap");
    private bool mediaPausa = false;

    private void OnEnable()
    {
        mat = GetComponent<Renderer>().sharedMaterial;
    }

#if UNITY_EDITOR

    private void OnWillRenderObject()
    {
        if (!EditorApplication.isPlaying)
        {
            if (medioRendimientoEditor)
            {
                if (mediaPausa)
                {
                    mediaPausa = false;
                }
                else
                {
                    mediaPausa = true;
                }

                if (mediaPausa)
                {
                    if (Offset1.x != 0 || Offset1.y != 0)
                    {
                        _offset1 = Offset1 * Time.realtimeSinceStartup;
                        mat.SetTextureOffset(uv1, _offset1);
                    }
                    if (Offset2.x != 0 || Offset2.y != 0)
                    {
                        _offset2 = Offset2 * Time.realtimeSinceStartup;
                        mat.SetTextureOffset(uv2, _offset2);
                    }
                }
            }
            else
            {
                if (Offset1.x != 0 || Offset1.y != 0)
                {
                    _offset1 = Offset1 * Time.realtimeSinceStartup;
                    mat.SetTextureOffset(uv1, _offset1);
                }
                if (Offset2.x != 0 || Offset2.y != 0)
                {
                    _offset2 = Offset2 * Time.realtimeSinceStartup;
                    mat.SetTextureOffset(uv2, _offset2);
                }
            }
        }
    }

#endif

    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            if (Offset1.x != 0 || Offset1.y != 0)
            {
                _offset1 += (Offset1 * Time.deltaTime);
                mat.SetTextureOffset(uv1, _offset1);
            }
            if (Offset2.x != 0 || Offset2.y != 0)
            {
                _offset2 += (Offset2 * Time.deltaTime);
                mat.SetTextureOffset(uv2, _offset2);
            }
        }
    }
}