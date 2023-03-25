namespace HannibalUI.Runtime.Base
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using com.voxelpixel.hannibal_ui.utils;
    using System;
    using HannibalUI.Runtime.Helpers.Observer;

    [RequireComponent(typeof(Canvas))]
    public abstract class VP_Canvas : VP_UnitySceneObject
    {
        [SerializeField]
        protected VP_UIObject[] uIObjects;

        protected CanvasType canvasType;
        protected Canvas panelCanvas;
        protected RectTransform panelRectTransform;
        protected Vector2 canvasSize;

        protected virtual void RegisterEvents() { }
        protected virtual void UnRegisterEvents() { }

        public abstract void Setup();

        protected ISubject<VP_UIEvent> _eventBroadcaster;

        public override void PreInit()
        {
            for (int i = 0; i < uIObjects.Length; i++)
            {
                uIObjects[i].PreInit();
            }
        }

        public void InjectSubject(ISubject<VP_UIEvent> subject)
        {
            _eventBroadcaster = subject;
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
            StopCoroutine("DeactivateWithAnimation");
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

            StartCoroutine("DeactivateWithAnimation", deactivationTime);
        }

        private IEnumerator DeactivateWithAnimation(float deactivationTime)
        {
            PlayDeactivateAnimations(deactivationTime);
            panelCanvas.sortingOrder = 2;
            yield return new WaitForSeconds(deactivationTime);
            Disable();
        }

        //TODO: This function should not be public! Solve this issue!
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
            UnRegisterEvents();

            foreach (var uiObject in uIObjects)
            {
                uiObject.OnUIObjectDestroy();
            }
        }
    }
}


