namespace com.voxelpixel.hannibal_ui.animation
{
    using UnityEngine;
    using DG.Tweening;

    public class ScaleAnimationComponent : AnimationComponent, IAnimable
    {
        public float ActivatedScale { get; set; }

        public float DeactivatedScale { get; set; }

        public ScaleAnimationComponent(RectTransform objectRectTransform) : base(objectRectTransform)
        {
            this.ObjectRectTransform = objectRectTransform;
        }

        public void PlayForward(float animationTime)
        {
            ObjectRectTransform.DOScale(ActivatedScale, animationTime).SetEase(AnimationEase);
        }

        public void PlayRewind(float animationTime)
        {
            ObjectRectTransform.DOScale(DeactivatedScale, animationTime).SetEase(AnimationEase);
        }
    }
}

