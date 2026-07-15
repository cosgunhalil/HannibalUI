namespace HannibalUI.Runtime.Animation
{
    using System;
    using DG.Tweening;
    using UnityEngine;

    /// <summary>
    /// Base for Inspector-authorable UI animations. Concrete subclasses are stored on a
    /// <c>VP_UIObject</c> via a <c>[SerializeReference]</c> list; the owning object injects the
    /// target <see cref="RectTransform"/> at init through <see cref="Bind"/> (it can't be a
    /// serialized field because it's a live scene reference, not authoring data).
    /// </summary>
    [Serializable]
    public abstract class AnimationComponent : IAnimable
    {
        [Tooltip("Easing curve applied to this animation.")]
        [SerializeField] private Ease _ease = Ease.InOutSine;

        protected RectTransform _rectTransform;

        protected Ease AnimationEase => _ease;

        public void Bind(RectTransform rectTransform)
        {
            _rectTransform = rectTransform;
        }

        public abstract void PlayForward(float activationTime);
        public abstract void PlayRewind(float deactivationTime);

        /// <summary>Snap the target to its activated end-state instantly (no tween).</summary>
        public abstract void ApplyActivated();

        /// <summary>Snap the target to its deactivated end-state instantly (no tween).</summary>
        public abstract void ApplyDeactivated();
    }
}
