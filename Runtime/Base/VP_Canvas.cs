namespace com.voxelpixel.hannibal_ui.base_component
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using com.voxelpixel.hannibal_ui.utils;

    [RequireComponent(typeof(Canvas))]
    public abstract class VP_Canvas : VP_UnitySceneObject
    {
        [SerializeField]
        protected VP_UIObject[] uIObjects;

        protected CanvasType canvasType;
        protected Canvas panelCanvas;
        protected RectTransform panelRectTransform;

        protected const float animationTime = .5f;

        protected virtual void RegisterEvents() { }
        protected virtual void UnRegisterEvents() { }

        public abstract void Setup();

        public override void PreInit()
        {
            SetAnimationTime(animationTime);

            for (int i = 0; i < uIObjects.Length; i++)
            {
                uIObjects[i].PreInit();
            }
        }

        private void SetAnimationTime(float animationTime)
        {
            for (int i = 0; i < uIObjects.Length; i++)
            {
                uIObjects[i].SetAnimationTime(animationTime);
            }
        }

        public override void Init()
        {
            panelCanvas = GetComponent<Canvas>();
            panelRectTransform = GetComponent<RectTransform>();
            Setup();
            var canvasSize = CanvasUtilities.GetCanvasSize(GetComponent<CanvasScaler>());
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

        public void Activate()
        {
            if (panelCanvas.enabled)
            {
                return;
            }

            Activateimmediately();
            StopCoroutine("DeactivateWithAnimation");
            PlayActivateAnimations();
        }

        public void Activateimmediately() 
        {
            panelCanvas.enabled = true;
            panelCanvas.sortingOrder = 1;
        }

        public void Deactivate()
        {
            if (!panelCanvas.enabled)
            {
                return;
            }

            StartCoroutine("DeactivateWithAnimation");
        }

        private IEnumerator DeactivateWithAnimation()
        {
            PlayDeactivateAnimations();
            panelCanvas.sortingOrder = 2;

            yield return new WaitForSeconds(animationTime);
            Deactivateimmediately();
        }

        //TODO: This function should not be public! Solve this issue!
        public void Deactivateimmediately()
        {
            panelCanvas.sortingOrder = 0;
            panelCanvas.enabled = false;
        }

        public void PlayActivateAnimations()
        {
            foreach (var uiObject in uIObjects)
            {
                uiObject.PlayActivateAnimation();
            }
        }

        public void PlayDeactivateAnimations()
        {
            foreach (var uiObject in uIObjects)
            {
                uiObject.PlayDeactivateAnimation();
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


