using DG.Tweening;

namespace HannibalUI.Runtime.Animation
{
    using UnityEngine;
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
            ObjectRectTransform.DOAnchorPos(ActivatedCoordinate, animationTime).SetEase(Ease.InOutSine);
        }

        public void PlayRewind(float animationTime)
        {
            ObjectRectTransform.DOAnchorPos(DeactivatedCoordinate, animationTime).SetEase(Ease.InOutSine);
        }
    }
}


