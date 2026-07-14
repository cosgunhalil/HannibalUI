namespace HannibalUI.Tests.Runtime
{
    using System.Collections;
    using HannibalUI.Runtime.Base;
    using NUnit.Framework;
    using UnityEngine.TestTools;

    public class NavigationServiceTests
    {
        private static VP_NavigationService CreateService(
            out FakeScreen main, out FakeScreen market, out FakeScreen characters)
        {
            main = new FakeScreen(CanvasType.Main);
            market = new FakeScreen(CanvasType.Market);
            characters = new FakeScreen(CanvasType.Characters);
            var registry = new VP_ScreenRegistry(new IScreen[] { main, market, characters });
            return new VP_NavigationService(registry, 0f);
        }

        [UnityTest]
        public IEnumerator Show_ActivatesScreen_AndStackHasOne()
        {
            var nav = CreateService(out var main, out _, out _);

            nav.Show(CanvasType.Main);
            yield return null;

            Assert.AreEqual(1, nav.StackCount);
            Assert.AreSame(main, nav.ActiveScreen);
            Assert.AreEqual(1, main.ActivateCount);
        }

        [UnityTest]
        public IEnumerator Push_GrowsStack_AndActivatesNewScreen()
        {
            var nav = CreateService(out var main, out var market, out _);

            nav.Show(CanvasType.Main);
            yield return null;
            nav.Push(CanvasType.Market);
            yield return null;

            Assert.AreEqual(2, nav.StackCount);
            Assert.AreSame(market, nav.ActiveScreen);
            Assert.AreEqual(1, main.DeactivateCount);
        }

        [UnityTest]
        public IEnumerator Pop_ReturnsToPreviousScreen()
        {
            var nav = CreateService(out var main, out _, out _);

            nav.Show(CanvasType.Main);
            yield return null;
            nav.Push(CanvasType.Market);
            yield return null;
            nav.Pop();
            yield return null;

            Assert.AreEqual(1, nav.StackCount);
            Assert.AreSame(main, nav.ActiveScreen);
        }

        [UnityTest]
        public IEnumerator Pop_AtRoot_IsNoOp()
        {
            var nav = CreateService(out var main, out _, out _);

            nav.Show(CanvasType.Main);
            yield return null;
            nav.Pop();
            yield return null;

            Assert.AreEqual(1, nav.StackCount);
            Assert.AreSame(main, nav.ActiveScreen);
        }

        [UnityTest]
        public IEnumerator PopTo_RemovesScreensAboveTarget()
        {
            var nav = CreateService(out var main, out _, out _);

            nav.Show(CanvasType.Main);
            yield return null;
            nav.Push(CanvasType.Market);
            yield return null;
            nav.Push(CanvasType.Characters);
            yield return null;
            nav.PopTo(CanvasType.Main);
            yield return null;

            Assert.AreEqual(1, nav.StackCount);
            Assert.AreSame(main, nav.ActiveScreen);
        }
    }
}
