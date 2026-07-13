namespace HannibalUI.Runtime.Base
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;
    using HannibalUI.Runtime.Utilities;

    [RequireComponent(typeof(Canvas))]
    public abstract class VP_Canvas : VP_UnitySceneObject
    {
        [SerializeField]
        protected VP_UIObject[] uIObjects;

        protected CanvasType canvasType;
        protected Canvas panelCanvas;
        protected RectTransform panelRectTransform;
        protected Vector2 canvasSize;
        private CancellationTokenSource _deactivationCts;

        protected virtual void RegisterEvents() { }
        protected virtual void UnRegisterEvents() { }

        public abstract void Setup();

        public event Action<UIEvents> UIEventRaised;

        public override void PreInit()
        {
            for (int i = 0; i < uIObjects.Length; i++)
            {
                uIObjects[i].PreInit();
            }
        }

        protected void RaiseUIEvent(UIEvents uiEvent)
        {
            UIEventRaised?.Invoke(uiEvent);
        }

        public override void Init()
        {
            panelCanvas = GetComponent<Canvas>();
            panelRectTransform = GetComponent<RectTransform>();
            Setup();
            canvasSize = CanvasUtilities.GetCanvasSize(GetComponent<CanvasScaler>());
            foreach (var uiObject in uIObjects)
            {
                uiObject.Init();
                uiObject.Setup(canvasSize);
            }
            RegisterEvents();
        }

        public override void LateInit()
        {
            foreach (var uiObject in uIObjects)
            {
                uiObject.LateInit();
            }
        }

        public void Activate(float activationTime)
        {
            if (panelCanvas.enabled)
            {
                return;
            }

            Enable();
            CancelPendingDeactivation();
            PlayActivateAnimations(activationTime);
        }

        private void Enable()
        {
            panelCanvas.enabled = true;
            panelCanvas.sortingOrder = 1;
        }

        public void Deactivate(float deactivationTime)
        {
            if (!panelCanvas.enabled)
            {
                return;
            }

            CancelPendingDeactivation();
            _deactivationCts = new CancellationTokenSource();
            DeactivateWithAnimationAsync(deactivationTime, _deactivationCts.Token);
        }

        private async UniTaskVoid DeactivateWithAnimationAsync(float deactivationTime, CancellationToken cancellationToken)
        {
            PlayDeactivateAnimations(deactivationTime);
            panelCanvas.sortingOrder = 2;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(deactivationTime), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            Disable();
        }

        private void CancelPendingDeactivation()
        {
            _deactivationCts?.Cancel();
            _deactivationCts?.Dispose();
            _deactivationCts = null;
        }

        private void Disable()
        {
            panelCanvas.sortingOrder = 0;
            panelCanvas.enabled = false;
        }

        public void PlayActivateAnimations(float activationTime)
        {
            foreach (var uiObject in uIObjects)
            {
                uiObject.PlayActivateAnimation(activationTime);
            }
        }

        public void PlayDeactivateAnimations(float deactivationTime)
        {
            foreach (var uiObject in uIObjects)
            {
                uiObject.PlayDeactivateAnimation(deactivationTime);
            }
        }

        public void OnDestroyCalled()
        {
            CancelPendingDeactivation();
            UnRegisterEvents();

            foreach (var uiObject in uIObjects)
            {
                uiObject.OnUIObjectDestroy();
            }
        }
    }
}


