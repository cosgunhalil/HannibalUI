namespace HannibalUI.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using HannibalUI.Editor.CodeGen;

    /// <summary>
    /// The HannibalUI authoring window: pick a UIProjectConfig, edit its screens, see validation
    /// status, and generate code. Transition editing and the flow diagram are added by later tasks.
    /// </summary>
    public class HannibalUIEditorWindow : EditorWindow
    {
        [SerializeField]
        private UIProjectConfig _config;

        private SerializedObject _serializedConfig;
        private VisualElement _body;
        private NavigationFlowView _flowView;
        private Label _statusLabel;
        private Button _generateButton;
        private Button _bootstrapButton;
        private Label _generateSummary;

        [MenuItem("HannibalUI/UI Editor")]
        public static void Open()
        {
            var window = GetWindow<HannibalUIEditorWindow>();
            window.titleContent = new GUIContent("HannibalUI");
            window.minSize = new Vector2(360f, 320f);
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
            configField.RegisterValueChangedCallback(evt => SetConfig(evt.newValue as UIProjectConfig));
            root.Add(configField);

            var scroll = new ScrollView { style = { flexGrow = 1f } };
            _body = new VisualElement { style = { marginTop = 6f } };
            scroll.Add(_body);
            root.Add(scroll);

            _statusLabel = new Label { style = { whiteSpace = WhiteSpace.Normal, marginTop = 6f, marginBottom = 6f } };
            root.Add(_statusLabel);

            _generateButton = new Button(OnGenerate) { text = "Generate" };
            root.Add(_generateButton);

            _bootstrapButton = new Button(OnBootstrap) { text = "Bootstrap Scene" };
            _bootstrapButton.style.marginTop = 2f;
            root.Add(_bootstrapButton);

            _generateSummary = new Label { style = { whiteSpace = WhiteSpace.Normal, marginTop = 4f } };
            root.Add(_generateSummary);

            SetConfig(_config);
        }

        private void SetConfig(UIProjectConfig config)
        {
            _config = config;
            _serializedConfig = _config != null ? new SerializedObject(_config) : null;

            if (_body == null)
            {
                return;
            }

            _body.Clear();

            if (_serializedConfig != null)
            {
                _body.Add(BuildScreensSection(_serializedConfig));
                _body.Add(BuildTransitionsSection(_serializedConfig));

                var flowHeader = new Label("Flow") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 6f } };
                _body.Add(flowHeader);
                _flowView = new NavigationFlowView();
                _body.Add(_flowView);
                _flowView.Rebuild(_config);

                _body.TrackSerializedObjectValue(_serializedConfig, _ =>
                {
                    UpdateStatus();
                    _flowView.Rebuild(_config);
                });
            }
            else
            {
                _flowView = null;
            }

            UpdateStatus();
        }

        private static VisualElement BuildScreensSection(SerializedObject serializedConfig)
        {
            var list = new ListView
            {
                showFoldoutHeader = true,
                headerTitle = "Screens",
                showAddRemoveFooter = true,
                reorderable = true,
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                bindingPath = "_screens"
            };
            list.Bind(serializedConfig);
            return list;
        }

        private VisualElement BuildTransitionsSection(SerializedObject serializedConfig)
        {
            var transitionsProp = serializedConfig.FindProperty("_transitions");

            var list = new ListView
            {
                showFoldoutHeader = true,
                headerTitle = "Transitions",
                showAddRemoveFooter = true,
                reorderable = true,
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                bindingPath = "_transitions"
            };

            // A single value-changed callback per row (registered here, not in bindItem) reads the
            // current property from userData, so recycled rows never stack stale callbacks.
            list.makeItem = () =>
            {
                var row = new VisualElement();
                row.style.marginBottom = 4f;
                row.style.paddingBottom = 4f;
                row.Add(new PropertyField { name = "event" });
                row.Add(new PropertyField { name = "action" });

                var target = new DropdownField("Target") { name = "target" };
                target.RegisterValueChangedCallback(evt =>
                {
                    if (target.userData is SerializedProperty targetProp)
                    {
                        targetProp.stringValue = evt.newValue;
                        targetProp.serializedObject.ApplyModifiedProperties();
                    }
                });
                row.Add(target);
                return row;
            };

            list.bindItem = (element, index) =>
            {
                var elementProp = transitionsProp.GetArrayElementAtIndex(index);
                element.Q<PropertyField>("event").BindProperty(elementProp.FindPropertyRelative("EventName"));
                element.Q<PropertyField>("action").BindProperty(elementProp.FindPropertyRelative("Action"));

                var target = element.Q<DropdownField>("target");
                var targetProp = elementProp.FindPropertyRelative("TargetScreen");
                target.userData = targetProp;
                target.choices = ScreenNames();
                target.SetValueWithoutNotify(targetProp.stringValue);
            };

            list.Bind(serializedConfig);
            return list;
        }

        private List<string> ScreenNames()
        {
            return ConfigQueries.NonPopupScreenNames(_config);
        }

        private void UpdateStatus()
        {
            if (_statusLabel == null)
            {
                return;
            }

            if (_config == null)
            {
                _statusLabel.text = "Select or create a UIProjectConfig to begin.";
                _generateButton.SetEnabled(false);
                _bootstrapButton.SetEnabled(false);
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

            _bootstrapButton.SetEnabled(SceneBootstrapper.IsGeneratedDirectorAvailable());
        }

        private void OnGenerate()
        {
            if (_config == null)
            {
                return;
            }

            var result = UICodeGenerator.Generate(_config);

            _generateSummary.text = result.Success
                ? $"Generated: {result.Created} created, {result.Overwritten} overwritten, {result.Skipped} skipped."
                : "Generation failed — see the status above and the Console.";

            if (result.Success)
            {
                PingGenerated();
            }

            UpdateStatus();
        }

        private void OnBootstrap()
        {
            if (_config == null)
            {
                return;
            }

            bool success = SceneBootstrapper.Bootstrap(_config);
            _generateSummary.text = success
                ? "Scene bootstrapped — press Play to try it."
                : "Bootstrap failed — see the Console.";
        }

        private void PingGenerated()
        {
            var path = System.IO.Path.Combine(_config.GeneratedFolder, "VP_GeneratedDirector.g.cs");
            var asset = AssetDatabase.LoadMainAssetAtPath(path);

            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
            }
        }
    }
}
