namespace HannibalUI.Runtime.Animation
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [Serializable]
    public class MoveAnimationComponent : AnimationComponent
    {
        [Tooltip("Anchored position when the object is shown (PlayForward target).")]
        [SerializeField] private Vector2 _activatedCoordinate;

        [Tooltip("Anchored position when hidden (PlayRewind target / starting offset).")]
        [SerializeField] private Vector2 _deactivatedCoordinate;

        public override void PlayForward(float activationTime)
        {
            _rectTransform.DOAnchorPos(_activatedCoordinate, activationTime).SetEase(AnimationEase);
        }

        public override void PlayRewind(float deactivationTime)
        {
            _rectTransform.DOAnchorPos(_deactivatedCoordinate, deactivationTime).SetEase(AnimationEase);
        }

        public override void ApplyActivated()
        {
            if (_rectTransform != null)
            {
                _rectTransform.anchoredPosition = _activatedCoordinate;
            }
        }

        public override void ApplyDeactivated()
        {
            if (_rectTransform != null)
            {
                _rectTransform.anchoredPosition = _deactivatedCoordinate;
            }
        }
    }
}
