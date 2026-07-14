namespace HannibalUI.Runtime.Base
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;
    using HannibalUI.Runtime.Utilities;

    [RequireComponent(typeof(Canvas))]
    public abstract class VP_Canvas : VP_UnitySceneObject, IScreen
    {
        [SerializeField]
        private CanvasType _canvasType;
        [SerializeField]
        protected VP_UIObject[] uIObjects;

        protected Canvas panelCanvas;
        protected RectTransform panelRectTransform;
        protected Vector2 canvasSize;

        public virtual CanvasType ScreenType => _canvasType;

        public event Action<UIEvents> UIEventRaised;

        protected virtual void RegisterEvents() { }
        protected virtual void UnRegisterEvents() { }

        public abstract void Setup();

        public override void PreInit()
        {
            for (int i = 0; i < uIObjects.Length; i++)
            {
                uIObjects[i].PreInit();
            }
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

        public async UniTask ActivateAsync(float duration, CancellationToken cancellationToken)
        {
            if (panelCanvas.enabled)
            {
                return;
            }

            Enable();
            PlayActivateAnimations(duration);

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Superseded mid-transition; the screen stays enabled for whoever took over.
            }
        }

        public async UniTask DeactivateAsync(float duration, CancellationToken cancellationToken)
        {
            if (!panelCanvas.enabled)
            {
                return;
            }

            PlayDeactivateAnimations(duration);
            panelCanvas.sortingOrder = 2;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return; // Re-activated mid-transition; leave the canvas visible.
            }

            Disable();
        }

        public void TearDown()
        {
            UnRegisterEvents();

            foreach (var uiObject in uIObjects)
            {
                uiObject.OnUIObjectDestroy();
            }
        }

        protected void RaiseUIEvent(UIEvents uiEvent)
        {
            UIEventRaised?.Invoke(uiEvent);
        }

        private void Enable()
        {
            panelCanvas.enabled = true;
            panelCanvas.sortingOrder = 1;
        }

        private void Disable()
        {
            panelCanvas.sortingOrder = 0;
            panelCanvas.enabled = false;
        }

        private void PlayActivateAnimations(float activationTime)
        {
            foreach (var uiObject in uIObjects)
            {
                uiObject.PlayActivateAnimation(activationTime);
            }
        }

        private void PlayDeactivateAnimations(float deactivationTime)
        {
            foreach (var uiObject in uIObjects)
            {
                uiObject.PlayDeactivateAnimation(deactivationTime);
            }
        }
    }
}
