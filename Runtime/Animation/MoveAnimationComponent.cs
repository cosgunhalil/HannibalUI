namespace HannibalUI.Runtime.Animation
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [Serializable]
    public class MoveAnimationComponent : AnimationComponent
    {
        [SerializeField] private Vector2 _activatedCoordinate;
        [SerializeField] private Vector2 _deactivatedCoordinate;

        public override void PlayForward(float activationTime)
        {
            _rectTransform.DOAnchorPos(_activatedCoordinate, activationTime).SetEase(AnimationEase);
        }

        public override void PlayRewind(float deactivationTime)
        {
            _rectTransform.DOAnchorPos(_deactivatedCoordinate, deactivationTime).SetEase(AnimationEase);
        }
    }
}
