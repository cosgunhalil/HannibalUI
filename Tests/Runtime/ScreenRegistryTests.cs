namespace HannibalUI.Tests.Runtime
{
    using System.Text.RegularExpressions;
    using HannibalUI.Runtime.Base;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class ScreenRegistryTests
    {
        [Test]
        public void Get_ResolvesScreenByType_IgnoringArrayOrder()
        {
            var main = new FakeScreen(CanvasType.Main);
            var market = new FakeScreen(CanvasType.Market);

            // Deliberately reversed relative to the enum order.
            var registry = new VP_ScreenRegistry(new IScreen[] { market, main });

            Assert.AreSame(main, registry.Get(CanvasType.Main));
            Assert.AreSame(market, registry.Get(CanvasType.Market));
        }

        [Test]
        public void TryGet_ReturnsFalse_ForUnregisteredType()
        {
            var registry = new VP_ScreenRegistry(new IScreen[] { new FakeScreen(CanvasType.Main) });

            Assert.IsTrue(registry.Contains(CanvasType.Main));
            Assert.IsFalse(registry.TryGet(CanvasType.Market, out _));
        }

        [Test]
        public void DuplicateType_KeepsFirst_AndLogsError()
        {
            var first = new FakeScreen(CanvasType.Main);
            var second = new FakeScreen(CanvasType.Main);

            LogAssert.Expect(LogType.Error, new Regex("duplicate screen"));
            var registry = new VP_ScreenRegistry(new IScreen[] { first, second });

            Assert.AreSame(first, registry.Get(CanvasType.Main));
        }
    }
}
