namespace com.voxelpixel.hannibal_ui.animation
{
    using UnityEngine;
    using DG.Tweening;
    public class MoveAnimationComponent : AnimationComponent, IAnimable
    {
        public Vector2 DeactivatedCoordinate { get; set; }
        public Vector2 ActivatedCoordinate { get; set; }


        public MoveAnimationComponent(RectTransform objectRectTransform, float animationTime) : base(objectRectTransform, animationTime)
        {
            this.ObjectRectTransform = objectRectTransform;
            this.AnimationTime = animationTime;
        }

        public void PlayForward()
        {
            //TODO: use do anchor position!
            ObjectRectTransform.DOMove(ActivatedCoordinate, AnimationTime).SetEase(Ease.InOutSine);
        }

        public void PlayRewind()
        {
            //TODO: use do anchor position!
            ObjectRectTransform.DOMove(DeactivatedCoordinate, AnimationTime).SetEase(Ease.InOutSine);
        }
    }
}


