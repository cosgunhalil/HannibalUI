//TODO: generate automatically!
namespace HannibalUI.Runtime.Base
{
    using UnityEngine;

    public class VP_Director : MonoBehaviour
    {
        private const float CANVAS_ACTIVATION_TIME = .5f;
        [SerializeField] private VP_Canvas[] canvases; // Order-independent: resolved by ScreenType through the registry.
        private VP_ScreenRegistry _registry;
        private VP_NavigationService _navigation;

        public VP_NavigationService Navigation => _navigation;

        public void Awake()
        {
            _registry = new VP_ScreenRegistry(canvases);
            _navigation = new VP_NavigationService(_registry, CANVAS_ACTIVATION_TIME);

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
            switch (uiEvent)
            {
                case UIEvents.ON_CHARACTERS_BUTTON_CLICK:
                    EnableCanvas(CanvasType.Characters);
                    break;
                case UIEvents.ON_MAIN_MENU_BUTTON_CLICK:
                    EnableCanvas(CanvasType.Main);
                    break;
                case UIEvents.ON_MARKET_BUTTON_CLICK:
                    EnableCanvas(CanvasType.Market);
                    break;
            }
        }
    }
}
