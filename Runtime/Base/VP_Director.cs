//TODO: generate automatically!
namespace HannibalUI.Runtime.Base
{
    using UnityEngine;

    public class VP_Director : MonoBehaviour
    {
        private const float CANVAS_ACTIVATION_TIME = .5f;
        [SerializeField] private VP_Canvas[] canvases; // Order-independent: resolved by ScreenType through the registry.
        [SerializeField] private NavRoute[] routes;    // UI event -> navigation action; hand-authored now, generated in Phase 2.
        private VP_ScreenRegistry _registry;
        private VP_NavigationService _navigation;
        private VP_UIEventRouter _router;

        public VP_NavigationService Navigation => _navigation;

        public void Awake()
        {
            _registry = new VP_ScreenRegistry(canvases);
            _navigation = new VP_NavigationService(_registry, CANVAS_ACTIVATION_TIME);
            _router = new VP_UIEventRouter(_navigation, routes);

            foreach (var canvas in canvases)
            {
                canvas.PreInit();
                canvas.UIEventRaised += HandleUIEvent;
            }
        }

        public void Start()
        {
            foreach (var canvas in canvases)
            {
                canvas.Init();
            }

            foreach (var canvas in canvases)
            {
                canvas.LateInit();
            }

            _navigation.Show(CanvasType.Main);
        }

        public void OnDestroy()
        {
            _navigation.Dispose();

            foreach (var canvas in canvases)
            {
                canvas.UIEventRaised -= HandleUIEvent;
                canvas.TearDown();
            }
        }

        public void EnableCanvas(CanvasType canvasType)
        {
            _navigation.Show(canvasType);
        }

        private void HandleUIEvent(UIEvents uiEvent)
        {
            _router.Handle(uiEvent);
        }
    }
}
