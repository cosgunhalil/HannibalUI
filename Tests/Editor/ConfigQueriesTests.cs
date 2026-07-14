namespace HannibalUI.Tests.Editor
{
    using System.Collections.Generic;
    using HannibalUI.Editor;
    using HannibalUI.Editor.CodeGen;
    using NUnit.Framework;

    public class ConfigQueriesTests
    {
        [Test]
        public void NonPopupScreenNames_ExcludesPopupsAndBlanks_PreservingOrder()
        {
            var config = ConfigTestFactory.Create(new List<ScreenDefinition>
            {
                new ScreenDefinition { Name = "Main", IsStart = true },
                new ScreenDefinition { Name = "Shop" },
                new ScreenDefinition { Name = "Confirm", IsPopup = true },
                new ScreenDefinition { Name = "" },
            });

            var names = ConfigQueries.NonPopupScreenNames(config);

            CollectionAssert.AreEqual(new[] { "Main", "Shop" }, names);
        }

        [Test]
        public void NonPopupScreenNames_NullConfig_ReturnsEmpty()
        {
            Assert.IsEmpty(ConfigQueries.NonPopupScreenNames(null));
        }
    }
}
