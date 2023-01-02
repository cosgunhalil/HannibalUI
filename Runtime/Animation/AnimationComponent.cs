namespace com.voxelpixel.hannibal_ui.animation
{
    using DG.Tweening;
    using UnityEngine;
    public class AnimationComponent 
    {
        public Ease AnimationEase { get; set; }

        protected RectTransform ObjectRectTransform;
        protected float AnimationTime;

        public AnimationComponent(RectTransform objectRectTransform, float animationTime) 
        {
            this.ObjectRectTransform = objectRectTransform;
            this.AnimationTime = animationTime;
        }
    }
}
