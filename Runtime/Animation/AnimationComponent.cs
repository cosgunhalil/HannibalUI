namespace HannibalUI.Runtime.Animation
{
    using DG.Tweening;
    using UnityEngine;
    public class AnimationComponent 
    {
        public Ease AnimationEase { get; set; }

        protected RectTransform ObjectRectTransform;

        public AnimationComponent(RectTransform objectRectTransform) 
        {
            this.ObjectRectTransform = objectRectTransform;
        }
    }
}
