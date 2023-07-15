//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEditor;

namespace DigitalOpus.MB.MBEditor
{
    [CustomEditor(typeof(MB3_TextureBaker))]
    [CanEditMultipleObjects]
    public class MB3_TextureBakerEditor : Editor
    {

        MB3_TextureBakerEditorInternal tbe = new MB3_TextureBakerEditorInternal();

        void OnEnable()
        {
            tbe.OnEnable(serializedObject);
        }

        void OnDisable()
        {
            tbe.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            tbe.DrawGUI(serializedObject, (MB3_TextureBaker)target, targets, typeof(MB3_MeshBakerEditorWindow));
        }

    }
}