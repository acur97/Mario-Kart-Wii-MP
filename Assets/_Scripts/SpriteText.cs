using UnityEngine;
using TMPro;
using System.Text;

#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(SpriteText))]
    class SpriteTextEditor : Editor
    {
        private const string Text1 = "Set Text";
        private const string Text2 = "Texto seteado";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SpriteText _SpriteText = (SpriteText)target;
            if (GUILayout.Button(Text1))
            {
                _SpriteText.SetText(_SpriteText._text);
                Debug.Log(Text2);
            }
        }
    }
#endif

[ExecuteInEditMode, RequireComponent(typeof (TextMeshProUGUI))]
public class SpriteText : MonoBehaviour
{
    public enum Fuente { turbo_PointNum, turbo_messageFontOutline, turbo_PointNum_Led }
    public Fuente _fuente;
    [TextArea(minLines:5, maxLines:10)]
    public string _text;

    private TextMeshProUGUI TMPtext;
    private SpriteTextAssets spriteTextAssets;

    private readonly string _1 = "<sprite name=\u0022";
    private readonly string _2 = "\u0022>";
    private readonly char _3 = ' ';
    private readonly char _4 = '"';
    private readonly string _4_1 = "_comillas";
    private readonly char _5 = '\n';
    private readonly char _6 = '\t';
    private readonly char _7 = '\r';

    private void Start()
    {
        TMPtext = GetComponent<TextMeshProUGUI>();
        spriteTextAssets = SpriteTextAssets._SpriteTextAssets;
        if (spriteTextAssets == null)
        {
            spriteTextAssets = FindObjectOfType<SpriteTextAssets>(true);
        }
        if (_text != null)
        {
            SetFont();
        }
    }

    public void SetText(string text)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == _3)
            {
                sb.Append(_3);
            }
            else if (text[i] == _4)
            {
                sb.Append(string.Concat(_1, _4_1, _2));
            }
            else if (text[i] == _5)
            {
                sb.AppendLine();
            }
            else if (text[i] == _6)
            {
                sb.Append(_6);
            }
            else if (text[i] == _7)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(string.Concat(_1, text[i], _2));
            }
            //<sprite name="1">
        }

        TMPtext.SetText(sb);

    #if UNITY_EDITOR
        SetFont();
        TMPtext.enabled = false;
        TMPtext.enabled = true;
    #endif
    }

    public void SetFont()
    {
        switch (_fuente)
        {
            case Fuente.turbo_messageFontOutline:
                TMPtext.spriteAsset = spriteTextAssets.turbo_messageFontOutline;
                TMPtext.characterSpacing = -35;
                TMPtext.wordSpacing = 25;
                TMPtext.lineSpacing = -135;
                break;
            case Fuente.turbo_PointNum:
                TMPtext.spriteAsset = spriteTextAssets.turbo_PointNum;
                TMPtext.characterSpacing = -35;
                TMPtext.wordSpacing = 25;
                TMPtext.lineSpacing = -135;
                break;
            case Fuente.turbo_PointNum_Led:
                TMPtext.spriteAsset = spriteTextAssets.turbo_PointNum_Led;
                TMPtext.characterSpacing = -5;
                TMPtext.wordSpacing = 0;
                TMPtext.lineSpacing = -112;
                break;
            default:
                break;
        }
    }
}