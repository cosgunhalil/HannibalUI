//TODO: generate automatically!
namespace HannibalUI.Runtime.Base
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class VP_Director : MonoBehaviour
    {
        private const float CANVAS_ACTIVATION_TIME = .5f;
        [SerializeField] private VP_Canvas[] canvases; // Order-independent: resolved by ScreenType through the registry.
        private VP_ScreenRegistry _registry;
        private IScreen activeScreen = null;
        private CancellationTokenSource _canvasSwitchCts;

        public void Awake()
        {
            _registry = new VP_ScreenRegistry(canvases);

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

            EnableCanvas(CanvasType.Main);
        }

        public void OnDestroy()
        {
            _canvasSwitchCts?.Cancel();
            _canvasSwitchCts?.Dispose();

            foreach (var canvas in canvases)
            {
                canvas.UIEventRaised -= HandleUIEvent;
                canvas.TearDown();
            }
        }

        public void EnableCanvas(CanvasType canvasType)
        {
            if (!_registry.TryGet(canvasType, out var targetScreen))
            {
                Debug.LogError($"VP_Director: no screen registered for '{canvasType}'.");
                return;
            }

            if (ReferenceEquals(activeScreen, targetScreen))
            {
                return;
            }

            _canvasSwitchCts?.Cancel();
            _canvasSwitchCts?.Dispose();
            _canvasSwitchCts = new CancellationTokenSource();
            SwitchScreenAsync(targetScreen, _canvasSwitchCts.Token);
        }

        private async UniTaskVoid SwitchScreenAsync(IScreen targetScreen, CancellationToken cancellationToken)
        {
            try
            {
                if (activeScreen != null)
                {
                    await activeScreen.DeactivateAsync(CANVAS_ACTIVATION_TIME, cancellationToken);
                }

                activeScreen = targetScreen;
                await targetScreen.ActivateAsync(CANVAS_ACTIVATION_TIME, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Superseded by a newer switch; that switch now owns the active-screen state.
            }
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
