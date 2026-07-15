namespace HannibalUI.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using HannibalUI.Runtime.Base;

    /// <summary>
    /// Design-time source of truth for a project's UI. The code generator reads this to emit the
    /// <c>CanvasType</c>/<c>UIEvents</c> enums, the navigation routes, and screen stubs. The
    /// runtime never reads this asset — it consumes the generated artifacts.
    /// </summary>
    [CreateAssetMenu(fileName = "UIProjectConfig", menuName = "HannibalUI/UI Project Config")]
    public class UIProjectConfig : ScriptableObject
    {
        [SerializeField]
        private List<ScreenDefinition> _screens = new List<ScreenDefinition>();

        [SerializeField]
        private List<TransitionDefinition> _transitions = new List<TransitionDefinition>();

        [Header("Generated output (project-relative paths)")]
        [Tooltip("Generator-owned files (enums, routes). Overwritten on every generate.")]
        [SerializeField]
        private string _generatedFolder = "Assets/HannibalUI/Runtime/Generated";

        [Tooltip("Screen stub scripts. Generated once per screen, then never overwritten.")]
        [SerializeField]
        private string _screensFolder = "Assets/HannibalUI/Runtime/Screens";

        [Tooltip("If on, bootstrap saves each screen as a prefab and places a connected instance in the scene.")]
        [SerializeField]
        private bool _saveScreensAsPrefabs;

        [Tooltip("Folder for generated screen prefabs (used when 'Save Screens As Prefabs' is on).")]
        [SerializeField]
        private string _prefabsFolder = "Assets/HannibalUI/Prefabs";

        public IReadOnlyList<ScreenDefinition> Screens => _screens;
        public IReadOnlyList<TransitionDefinition> Transitions => _transitions;
        public string GeneratedFolder => _generatedFolder;
        public string ScreensFolder => _screensFolder;
        public bool SaveScreensAsPrefabs => _saveScreensAsPrefabs;
        public string PrefabsFolder => _prefabsFolder;
    }

    /// <summary>One screen (canvas) in the project. Its name becomes a <c>CanvasType</c> member.</summary>
    [Serializable]
    public class ScreenDefinition
    {
        public string Name;
        public bool IsStart;
        public bool IsPopup;
    }

    /// <summary>
    /// "When <see cref="EventName"/> is raised, perform <see cref="Action"/> toward
    /// <see cref="TargetScreen"/>." Generates both a <c>UIEvents</c> member and a <c>NavRoute</c>.
    /// <see cref="TargetScreen"/> is ignored for <see cref="NavActionType.Pop"/>/<see cref="NavActionType.Back"/>.
    /// </summary>
    [Serializable]
    public class TransitionDefinition
    {
        public string EventName;
        public NavActionType Action;
        public string TargetScreen;
    }
}
