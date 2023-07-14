using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class SpriteTextAssets : MonoBehaviour
{
    public static SpriteTextAssets _SpriteTextAssets;

    public TMP_SpriteAsset turbo_messageFontOutline;
    public TMP_SpriteAsset turbo_PointNum;
    public TMP_SpriteAsset turbo_PointNum_Led;

    private object[] single;

    private void Awake()
    {
        if (!(Application.isEditor && !Application.isPlaying))
        {
            single = FindObjectsOfType(typeof(SpriteTextAssets));
            int len = single.Length;
            if (len == 2)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
        }

        _SpriteTextAssets = this;
    }
}