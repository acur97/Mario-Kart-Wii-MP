using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePixelOffset : MonoBehaviour
{
    public float espaciado = 0.1f;
    private float _espaciado = 0.1f;
    [Header("Steps")]
    public Vector2 Offset1 = new Vector2(0.03125f, 0);
    private Vector2 _offset1 = Vector2.zero;
    //public Vector2 Offset2 = Vector2.zero;
    //private Vector2 _offset2 = Vector2.zero;

    private Material mat;
    private int uv1 = Shader.PropertyToID("_MainTex");
    //private int uv2 = Shader.PropertyToID("_DetailAlbedoMap");

    private void Awake()
    {
        mat = GetComponent<Renderer>().sharedMaterial;
        _espaciado = espaciado;
    }

    private void LateUpdate()
    {
        _espaciado -= Time.deltaTime;
        if (_espaciado <= 0)
        {
            _espaciado = espaciado;
            //offset´ear

            if (Offset1.x != 0 || Offset1.y != 0)
            {
                _offset1 += Offset1;
                mat.SetTextureOffset(uv1, _offset1);
            }
            //if (Offset2.x != 0 || Offset2.y != 0)
            //{
            //    _offset2 += Offset2;
            //    mat.SetTextureOffset(uv2, _offset2);
            //}

            if (_offset1.x == 1)
            {
                _offset1.x = 0;
            }
            if (_offset1.y == 1)
            {
                _offset1.y = 0;
            }
            //if (_offset2.x == 1)
            //{
            //    _offset2.x = 0;
            //}
            //if (_offset2.y == 1)
            //{
            //    _offset2.y = 0;
            //}
        }
    }
}