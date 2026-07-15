namespace HannibalUI.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using HannibalUI.Editor.CodeGen;
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
            }

            var canvasComponents = new List<Component>();
            foreach (var screen in config.Screens)
            {
                if (screen.IsPopup)
                {
                    continue;
                }

                var canvasComponent = CreateOrReuseScreenCanvas(screen, directorObject.transform, config);
                if (canvasComponent != null)
                {
                    canvasComponents.Add(canvasComponent);
                }
            }

            var canvasesProperty = serializedDirector.FindProperty("canvases");
            if (canvasesProperty != null)
            {
                canvasesProperty.arraySize = canvasComponents.Count;
                for (int i = 0; i < canvasComponents.Count; i++)
                {
                    canvasesProperty.GetArrayElementAtIndex(i).objectReferenceValue = canvasComponents[i];
                }
            }

            serializedDirector.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Selection.activeGameObject = directorObject;

            Debug.Log($"HannibalUI: scene ready — EventSystem, Director, PopupLayer, and " +
                      $"{canvasComponents.Count} screen canvas(es).");
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

        private static Component CreateOrReuseScreenCanvas(ScreenDefinition screen, Transform parent, UIProjectConfig config)
        {
            var className = ScreenStubGenerator.ClassName(screen);
            var screenType = ResolveScreenType(className);
            if (screenType == null)
            {
                Debug.LogError($"HannibalUI: screen type '{className}' not found — run Generate first.");
                return null;
            }

            var existing = GameObject.Find(className);
            if (existing != null)
            {
                return EnsureScreenComponent(existing, screenType);
            }

            if (config.SaveScreensAsPrefabs)
            {
                var prefabPath = System.IO.Path.Combine(config.PrefabsFolder, className + ".prefab");
                var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefabAsset != null)
                {
                    var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset, parent);
                    Undo.RegisterCreatedObjectUndo(instance, "Instantiate Screen Prefab");
                    return EnsureScreenComponent(instance, screenType);
                }
            }

            var screenObject = BuildScreenObject(className, parent);
            var component = EnsureScreenComponent(screenObject, screenType);

            if (config.SaveScreensAsPrefabs)
            {
                EnsureFolder(config.PrefabsFolder);
                var prefabPath = System.IO.Path.Combine(config.PrefabsFolder, className + ".prefab");
                PrefabUtility.SaveAsPrefabAssetAndConnect(screenObject, prefabPath, InteractionMode.UserAction);
            }

            return component;
        }

        private static GameObject BuildScreenObject(string name, Transform parent)
        {
            var screenObject = new GameObject(
                name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            screenObject.transform.SetParent(parent, false);

            var canvas = screenObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.enabled = false; // hidden until navigated to; the director shows the start screen.

            var scaler = screenObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            Undo.RegisterCreatedObjectUndo(screenObject, "Create Screen Canvas");
            return screenObject;
        }

        private static Component EnsureScreenComponent(GameObject screenObject, System.Type screenType)
        {
            var component = screenObject.GetComponent(screenType);
            if (component == null)
            {
                component = Undo.AddComponent(screenObject, screenType);
            }

            return component;
        }

        private static void EnsureFolder(string folder)
        {
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
        }

        private static System.Type ResolveScreenType(string className)
        {
            foreach (var type in TypeCache.GetTypesDerivedFrom<VP_Canvas>())
            {
                if (!type.IsAbstract && type.Name == className)
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
