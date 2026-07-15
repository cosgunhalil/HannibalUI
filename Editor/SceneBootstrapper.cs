namespace HannibalUI.Editor
{
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using HannibalUI.Runtime.Base;

    /// <summary>
    /// Builds the runtime scaffolding a generated UI needs, in the currently open scene: an
    /// EventSystem, the generated director GameObject, and a popup layer. Requires code generation
    /// to have run first (the generated director type must exist). Per-screen canvases come later.
    /// </summary>
    public static class SceneBootstrapper
    {
        private const string DirectorObjectName = "HannibalUI Director";
        private const string PopupLayerObjectName = "PopupLayer";
        private const int PopupSortingOrder = 100;

        [MenuItem("HannibalUI/Bootstrap Scene From Config")]
        private static void BootstrapFromSelection()
        {
            Bootstrap(Selection.activeObject as UIProjectConfig);
        }

        public static bool Bootstrap(UIProjectConfig config)
        {
            if (config == null)
            {
                Debug.LogError("HannibalUI: select a UIProjectConfig to bootstrap from.");
                return false;
            }

            var directorType = ResolveGeneratedDirectorType();
            if (directorType == null)
            {
                Debug.LogError("HannibalUI: VP_GeneratedDirector not found — run Generate before bootstrapping.");
                return false;
            }

            EnsureEventSystem();

            var directorObject = GameObject.Find(DirectorObjectName);
            if (directorObject == null)
            {
                directorObject = new GameObject(DirectorObjectName);
                Undo.RegisterCreatedObjectUndo(directorObject, "Create HannibalUI Director");
            }

            var director = directorObject.GetComponent(directorType);
            if (director == null)
            {
                director = Undo.AddComponent(directorObject, directorType);
            }

            var popupLayer = EnsurePopupLayer();

            var serializedDirector = new SerializedObject(director);
            var popupLayerProperty = serializedDirector.FindProperty("popupLayer");
            if (popupLayerProperty != null)
            {
                popupLayerProperty.objectReferenceValue = popupLayer.transform;
                serializedDirector.ApplyModifiedProperties();
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Selection.activeGameObject = directorObject;

            Debug.Log("HannibalUI: scene scaffolding ready (EventSystem, Director, PopupLayer). " +
                      "Add per-screen canvases next.");
            return true;
        }

        private static System.Type ResolveGeneratedDirectorType()
        {
            foreach (var type in TypeCache.GetTypesDerivedFrom<VP_Director>())
            {
                if (!type.IsAbstract && type.Name == "VP_GeneratedDirector")
                {
                    return type;
                }
            }

            return null;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
        }

        private static GameObject EnsurePopupLayer()
        {
            var existing = GameObject.Find(PopupLayerObjectName);
            if (existing != null)
            {
                return existing;
            }

            var popupLayer = new GameObject(
                PopupLayerObjectName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

            var canvas = popupLayer.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = PopupSortingOrder;

            Undo.RegisterCreatedObjectUndo(popupLayer, "Create PopupLayer");
            return popupLayer;
        }
    }
}
