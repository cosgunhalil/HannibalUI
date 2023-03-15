namespace com.voxelpixel.hannibal_ui.base_component
{
    using System.Collections.Generic;
    using UnityEngine;
    using com.voxelpixel.hannibal_ui.animation;
    public class VP_UIObject : VP_UnitySceneObject
    {
        protected RectTransform ObjectRectTransform;
        protected List<IAnimable> AnimationComponents;
        private float animationTime;

        public override void Init()
        {
            AnimationComponents = new List<IAnimable>();
            ObjectRectTransform = GetComponent<RectTransform>();
        }

        public virtual void Setup(Vector2 canvasSize) { }
        public void PlayActivateAnimation(float activationTime) 
        {
            if (ObjectRectTransform == null) 
            {
                Debug.LogError("Object transfor is null!" + gameObject.name);
                return;
            }

            foreach (var animationComponent in AnimationComponents)
            {
                animationComponent.PlayForward(activationTime);
            }
        }

        public void PlayDeactivateAnimation(float deactivationTime)
        {
            if (ObjectRectTransform == null)
            {
                Debug.LogError("Object transfor is null!" + gameObject.name);
                return;
            }

            foreach (var animationComponent in AnimationComponents)
            {
                animationComponent.PlayRewind(deactivationTime);
            }

        }

        public override void LateInit()
        {
            
        }

        public virtual void SetSize(Vector2 size)
        {

        }

        public void SetWidth(float width, bool preserveAspect)
        {
            if (preserveAspect)
            {
                var sizeDelta = ObjectRectTransform.sizeDelta;
                var currentHeight = sizeDelta.y;
                var currentWidth = sizeDelta.x;
                var futureHeight = (currentHeight * width) / currentWidth;

                sizeDelta = new Vector2(width, futureHeight);
                ObjectRectTransform.sizeDelta = sizeDelta;
            }
            else
            {
                ObjectRectTransform.sizeDelta = new Vector2(width, ObjectRectTransform.sizeDelta.y);
            }

        }

        public void SetHeight(float height, bool preserveAspect)
        {
            if (preserveAspect)
            {
                var sizeDelta = ObjectRectTransform.sizeDelta;
                var currentHeight = sizeDelta.y;
                var currentWidth = sizeDelta.x;
                var futureWidth = (currentWidth * height) / currentHeight;

                sizeDelta = new Vector2(futureWidth, height);
                ObjectRectTransform.sizeDelta = sizeDelta;
            }
            else
            {
                ObjectRectTransform.sizeDelta = new Vector2(ObjectRectTransform.sizeDelta.x, height);
            }

        }

        public virtual void SetPosition(float width)
        {

        }

        public void SetAnchorMax(Vector2 anchorMax)
        {
            ObjectRectTransform.anchorMax = anchorMax;
        }

        public void SetAnchorMin(Vector2 anchorMin)
        {
            ObjectRectTransform.anchorMin = anchorMin;
        }

        public void SetAnchorPosition(Vector2 position)
        {
            ObjectRectTransform.anchoredPosition = position;
        }

        public virtual void OnUIObjectDestroy()
        {
            //TODO: handle!
        }

        public Vector2 GetSizeDelta()
        {
            return ObjectRectTransform.sizeDelta;
        }

        public Vector2 GetRectSize()
        {
            return ObjectRectTransform.rect.size;
        }

        public Vector2 GetAnchoredPosition()
        {
            return ObjectRectTransform.anchoredPosition;
        }
        
    }
}

