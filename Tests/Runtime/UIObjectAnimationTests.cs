namespace HannibalUI.Tests.Runtime
{
    using HannibalUI.Runtime.Animation;
    using HannibalUI.Runtime.Base;
    using NUnit.Framework;
    using UnityEngine;

    public class UIObjectAnimationTests
    {
        // Records lifecycle calls without touching DOTween.
        private class ProbeAnimation : AnimationComponent
        {
            public int ForwardCount;
            public int RewindCount;
            public RectTransform BoundTransform => _rectTransform;

            public override void PlayForward(float activationTime)
            {
                ForwardCount++;
            }

            public override void PlayRewind(float deactivationTime)
            {
                RewindCount++;
            }
        }

        // Exposes the protected serialized list so a test can populate it without the Inspector.
        private class ProbeUIObject : VP_UIObject
        {
            public void AddAnimation(AnimationComponent animation)
            {
                _animationComponents.Add(animation);
            }
        }

        private GameObject _go;

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
            {
                Object.DestroyImmediate(_go);
            }
        }

        [Test]
        public void Init_BindsAnimationsToOwnRectTransform()
        {
            _go = new GameObject("probe", typeof(RectTransform));
            var uiObject = _go.AddComponent<ProbeUIObject>();
            var animation = new ProbeAnimation();
            uiObject.AddAnimation(animation);

            uiObject.Init();

            Assert.AreSame(_go.GetComponent<RectTransform>(), animation.BoundTransform);
        }

        [Test]
        public void PlayAnimations_InvokeForwardThenRewind()
        {
            _go = new GameObject("probe", typeof(RectTransform));
            var uiObject = _go.AddComponent<ProbeUIObject>();
            var animation = new ProbeAnimation();
            uiObject.AddAnimation(animation);
            uiObject.Init();

            uiObject.PlayActivateAnimation(0.1f);
            uiObject.PlayDeactivateAnimation(0.1f);

            Assert.AreEqual(1, animation.ForwardCount);
            Assert.AreEqual(1, animation.RewindCount);
        }
    }
}
