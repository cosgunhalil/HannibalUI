//TODO: generate automatically! 
namespace HannibalUI.Runtime.Base
{
    using HannibalUI.Runtime.Helpers.Observer;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class VP_Director : MonoBehaviour, Helpers.Observer.IObserver<VP_UIEvent>
    {
        private const float CANVAS_ACTIVATION_TIME = .5f;
        [SerializeField]private VP_Canvas[] canvases;//TODO: solve the order issue! Order is really important in here!
        private VP_Canvas activeCanvas = null;
        private VP_EventBroadcaster _eventBroadcaster;
        private CancellationTokenSource _canvasSwitchCts;

        public void Awake()
        {
            _eventBroadcaster = new VP_EventBroadcaster();
            _eventBroadcaster.Register(this);

            foreach (var canvas in canvases)
            {
                canvas.PreInit();
                canvas.InjectSubject(_eventBroadcaster);
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
                canvas.OnDestroyCalled();
            }

            _eventBroadcaster.UnRegister(this);
        }

        public void EnableCanvas(CanvasType canvasType)
        {
            if (canvases.Length == 0)
            {
                return;
            }

            var targetCanvas = canvases[(int)canvasType];

            if (activeCanvas == null)
            {
                activeCanvas = targetCanvas;
                activeCanvas.Activate(CANVAS_ACTIVATION_TIME);
                return;
            }

            if (activeCanvas == targetCanvas) 
            {
                return;
            }

            _canvasSwitchCts?.Cancel();
            _canvasSwitchCts?.Dispose();
            _canvasSwitchCts = new CancellationTokenSource();
            EnableRequestedCanvasAsync(targetCanvas, _canvasSwitchCts.Token);
        }

        private async UniTaskVoid EnableRequestedCanvasAsync(VP_Canvas targetCanvas, CancellationToken cancellationToken)
        {
            if (activeCanvas != null)
            {
                activeCanvas.Deactivate(CANVAS_ACTIVATION_TIME);
            }

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(CANVAS_ACTIVATION_TIME), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            activeCanvas = targetCanvas;
            activeCanvas.Activate(CANVAS_ACTIVATION_TIME);
        }

        public void Notify(object sender, VP_UIEvent eventArgs)
        {
            switch (eventArgs.GetEventType()) 
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