namespace HannibalUI.Runtime.Base
{
    using System.Collections.Generic;
    using UnityEngine;
    using HannibalUI.Runtime.Animation;
    public class VP_UIObject : VP_UnitySceneObject
    {
        protected RectTransform ObjectRectTransform;

        [SerializeReference]
        [SerializeField]
        protected List<AnimationComponent> _animationComponents = new List<AnimationComponent>();

        public override void Init()
        {
            ObjectRectTransform = GetComponent<RectTransform>();

            if (_animationComponents == null)
            {
                _animationComponents = new List<AnimationComponent>();
            }

            foreach (var animationComponent in _animationComponents)
            {
                animationComponent?.Bind(ObjectRectTransform);
            }
        }

        public virtual void Setup(Vector2 canvasSize) { }
        public void PlayActivateAnimation(float activationTime) 
        {
            if (ObjectRectTransform == null) 
            {
                Debug.LogError("Object transfor is null!" + gameObject.name);
                return;
            }

            foreach (var animationComponent in _animationComponents)
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

            foreach (var animationComponent in _animationComponents)
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

