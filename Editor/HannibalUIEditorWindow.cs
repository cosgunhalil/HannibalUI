namespace HannibalUI.Editor
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using HannibalUI.Editor.CodeGen;

    /// <summary>
    /// The HannibalUI authoring window: pick a UIProjectConfig, see its validation status, and
    /// generate code. Screen/transition editing and the flow diagram are added by later tasks.
    /// </summary>
    public class HannibalUIEditorWindow : EditorWindow
    {
        [SerializeField]
        private UIProjectConfig _config;

        private Label _statusLabel;
        private Button _generateButton;

        [MenuItem("HannibalUI/UI Editor")]
        public static void Open()
        {
            var window = GetWindow<HannibalUIEditorWindow>();
            window.titleContent = new GUIContent("HannibalUI");
            window.minSize = new Vector2(360f, 240f);
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingLeft = 8f;
            root.style.paddingRight = 8f;
            root.style.paddingTop = 8f;
            root.style.paddingBottom = 8f;

            var title = new Label("HannibalUI — UI Editor");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 14f;
            title.style.marginBottom = 6f;
            root.Add(title);

            var configField = new ObjectField("Config")
            {
                objectType = typeof(UIProjectConfig),
                allowSceneObjects = false,
                value = _config
            };
            configField.RegisterValueChangedCallback(evt =>
            {
                _config = evt.newValue as UIProjectConfig;
                Refresh();
            });
            root.Add(configField);

            _statusLabel = new Label { style = { whiteSpace = WhiteSpace.Normal, marginTop = 6f, marginBottom = 6f } };
            root.Add(_statusLabel);

            _generateButton = new Button(OnGenerate) { text = "Generate" };
            root.Add(_generateButton);

            Refresh();
        }

        private void Refresh()
        {
            if (_statusLabel == null)
            {
                return;
            }

            if (_config == null)
            {
                _statusLabel.text = "Select or create a UIProjectConfig to begin.";
                _generateButton.SetEnabled(false);
                return;
            }

            var errors = ConfigValidator.Validate(_config);

            if (errors.Count == 0)
            {
                _statusLabel.text = $"Valid — {_config.Screens.Count} screen(s), {_config.Transitions.Count} transition(s).";
                _generateButton.SetEnabled(true);
            }
            else
            {
                _statusLabel.text = "Fix before generating:\n• " + string.Join("\n• ", errors);
                _generateButton.SetEnabled(false);
            }
        }

        private void OnGenerate()
        {
            if (_config == null)
            {
                return;
            }

            UICodeGenerator.Generate(_config);
            Refresh();
        }
    }
}
