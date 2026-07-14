namespace HannibalUI.Editor
{
    using UnityEditor;
    using UnityEngine;
    using HannibalUI.Editor.CodeGen;

    [CustomEditor(typeof(UIProjectConfig))]
    public class UIProjectConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _screens;
        private SerializedProperty _transitions;
        private SerializedProperty _generatedFolder;
        private SerializedProperty _screensFolder;

        private void OnEnable()
        {
            _screens = serializedObject.FindProperty("_screens");
            _transitions = serializedObject.FindProperty("_transitions");
            _generatedFolder = serializedObject.FindProperty("_generatedFolder");
            _screensFolder = serializedObject.FindProperty("_screensFolder");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_screens, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_transitions, true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_generatedFolder);
            EditorGUILayout.PropertyField(_screensFolder);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            DrawValidationAndGenerate();
        }

        private void DrawValidationAndGenerate()
        {
            var config = (UIProjectConfig)target;
            var errors = ConfigValidator.Validate(config);

            if (errors.Count == 0)
            {
                EditorGUILayout.HelpBox("Config is valid.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Fix before generating:\n• " + string.Join("\n• ", errors),
                    MessageType.Error);
            }

            using (new EditorGUI.DisabledScope(errors.Count > 0))
            {
                if (GUILayout.Button("Generate"))
                {
                    UICodeGenerator.Generate(config);
                }
            }
        }
    }
}
