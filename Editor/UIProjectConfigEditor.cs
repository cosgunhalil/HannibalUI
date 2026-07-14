namespace HannibalUI.Editor
{
    using UnityEditor;
    using UnityEngine;
    using HannibalUI.Editor.CodeGen;

    [CustomEditor(typeof(UIProjectConfig))]
    public class UIProjectConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                UICodeGenerator.Generate((UIProjectConfig)target);
            }
        }
    }
}
