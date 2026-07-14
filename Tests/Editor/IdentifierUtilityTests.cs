namespace HannibalUI.Tests.Editor
{
    using HannibalUI.Editor.CodeGen;
    using NUnit.Framework;

    public class IdentifierUtilityTests
    {
        [TestCase("Main Menu", "MainMenu")]
        [TestCase("shop", "Shop")]
        [TestCase("in-game hud", "InGameHud")]
        [TestCase("2p", "_2p")]
        [TestCase("", "_")]
        public void ToPascalCase_ProducesExpectedIdentifier(string input, string expected)
        {
            Assert.AreEqual(expected, IdentifierUtility.ToPascalCase(input));
        }

        [TestCase("ON_MARKET_BUTTON_CLICK", "ON_MARKET_BUTTON_CLICK")]
        [TestCase("market  click", "market_click")]
        [TestCase("123", "_123")]
        [TestCase("  spaced  ", "spaced")]
        public void ToValidIdentifier_ProducesExpectedIdentifier(string input, string expected)
        {
            Assert.AreEqual(expected, IdentifierUtility.ToValidIdentifier(input));
        }
    }
}
