namespace com.voxelpixel.hannibal_ui.animation
{
    using UnityEngine;
    using DG.Tweening;

    public class ScaleAnimationComponent : AnimationComponent, IAnimable
    {
        public float ActivatedScale { get; set; }

        public float DeactivatedScale { get; set; }

        public ScaleAnimationComponent(RectTransform objectRectTransform, float animationTime) : base(objectRectTransform, animationTime)
        {
            this.ObjectRectTransform = objectRectTransform;
            this.AnimationTime = animationTime;
        }

        public void PlayForward()
        {
            ObjectRectTransform.DOScale(ActivatedScale, AnimationTime).SetEase(AnimationEase);
        }

        public void PlayRewind()
        {
            ObjectRectTransform.DOScale(DeactivatedScale, AnimationTime).SetEase(AnimationEase);
        }
    }
}

