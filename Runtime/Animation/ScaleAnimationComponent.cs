namespace HannibalUI.Runtime.Animation
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [Serializable]
    public class ScaleAnimationComponent : AnimationComponent
    {
        [Tooltip("Local scale when the object is shown.")]
        [SerializeField] private float _activatedScale = 1f;

        [Tooltip("Local scale when hidden.")]
        [SerializeField] private float _deactivatedScale;

        public override void PlayForward(float activationTime)
        {
            _rectTransform.DOScale(_activatedScale, activationTime).SetEase(AnimationEase);
        }

        public override void PlayRewind(float deactivationTime)
        {
            _rectTransform.DOScale(_deactivatedScale, deactivationTime).SetEase(AnimationEase);
        }

        public override void ApplyActivated()
        {
            if (_rectTransform != null)
            {
                _rectTransform.localScale = Vector3.one * _activatedScale;
            }
        }

        public override void ApplyDeactivated()
        {
            if (_rectTransform != null)
            {
                _rectTransform.localScale = Vector3.one * _deactivatedScale;
            }
        }
    }
}
