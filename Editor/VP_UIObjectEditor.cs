namespace HannibalUI.Editor
{
    using UnityEditor;
    using UnityEngine;
    using HannibalUI.Runtime.Base;

    /// <summary>
    /// Adds edit-mode "Preview Activated/Deactivated" buttons to every VP_UIObject so authored
    /// animation poses can be checked without entering Play mode.
    /// </summary>
    [CustomEditor(typeof(VP_UIObject), true)]
    public class VP_UIObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var uiObject = (VP_UIObject)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animation preview (edit mode)", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Preview Activated"))
                {
                    ApplyPreview(uiObject, true);
                }

                if (GUILayout.Button("Preview Deactivated"))
                {
                    ApplyPreview(uiObject, false);
                }
            }
        }

        private static void ApplyPreview(VP_UIObject uiObject, bool activated)
        {
            var rectTransform = uiObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Undo.RecordObject(rectTransform, "Preview Animation");
            }

            uiObject.EditorPreviewAnimations(activated);
        }
    }
}
