namespace HannibalUI.Runtime.Animation
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    [Serializable]
    public class ScaleAnimationComponent : AnimationComponent
    {
        [SerializeField] private float _activatedScale = 1f;
        [SerializeField] private float _deactivatedScale;

        public override void PlayForward(float activationTime)
        {
            _rectTransform.DOScale(_activatedScale, activationTime).SetEase(AnimationEase);
        }

        public override void PlayRewind(float deactivationTime)
        {
            _rectTransform.DOScale(_deactivatedScale, deactivationTime).SetEase(AnimationEase);
        }
    }
}
