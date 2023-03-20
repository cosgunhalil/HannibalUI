namespace com.voxelpixel.hannibal_ui.animation
{
    using UnityEngine;
    using DG.Tweening;
    public class MoveAnimationComponent : AnimationComponent, IAnimable
    {
        public Vector2 DeactivatedCoordinate { get; set; }
        public Vector2 ActivatedCoordinate { get; set; }


        public MoveAnimationComponent(RectTransform objectRectTransform) : base(objectRectTransform)
        {
            this.ObjectRectTransform = objectRectTransform;
        }

        public void PlayForward(float animationTime)
        {
            //TODO: use do anchor position!
            ObjectRectTransform.DOAnchorPos(ActivatedCoordinate, animationTime).SetEase(Ease.InOutSine);
        }

        public void PlayRewind(float animationTime)
        {
            //TODO: use do anchor position!
            ObjectRectTransform.DOAnchorPos(DeactivatedCoordinate, animationTime).SetEase(Ease.InOutSine);
        }
    }
}


