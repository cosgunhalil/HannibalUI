namespace HannibalUI.Tests.Editor
{
    using System.Collections.Generic;
    using HannibalUI.Editor;
    using HannibalUI.Editor.CodeGen;
    using HannibalUI.Runtime.Base;
    using NUnit.Framework;

    public class ConfigValidatorTests
    {
        private static List<ScreenDefinition> OneStartScreen()
        {
            return new List<ScreenDefinition> { new ScreenDefinition { Name = "Main", IsStart = true } };
        }

        [Test]
        public void ValidConfig_HasNoErrors()
        {
            var config = ConfigTestFactory.Create(
                new List<ScreenDefinition>
                {
                    new ScreenDefinition { Name = "Main", IsStart = true },
                    new ScreenDefinition { Name = "Shop" },
                },
                new List<TransitionDefinition>
                {
                    new TransitionDefinition { EventName = "ON_SHOP_CLICK", Action = NavActionType.Show, TargetScreen = "Shop" },
                });

            Assert.IsEmpty(ConfigValidator.Validate(config));
        }

        [Test]
        public void DuplicateScreen_IsReported()
        {
            var config = ConfigTestFactory.Create(new List<ScreenDefinition>
            {
                new ScreenDefinition { Name = "Main", IsStart = true },
                new ScreenDefinition { Name = "Main" },
            });

            Assert.IsNotEmpty(ConfigValidator.Validate(config));
        }

        [Test]
        public void NoStartScreen_IsReported()
        {
            var config = ConfigTestFactory.Create(new List<ScreenDefinition>
            {
                new ScreenDefinition { Name = "Main" },
            });

            CollectionAssert.IsNotEmpty(ConfigValidator.Validate(config));
        }

        [Test]
        public void MultipleStartScreens_IsReported()
        {
            var config = ConfigTestFactory.Create(new List<ScreenDefinition>
            {
                new ScreenDefinition { Name = "Main", IsStart = true },
                new ScreenDefinition { Name = "Shop", IsStart = true },
            });

            Assert.IsNotEmpty(ConfigValidator.Validate(config));
        }

        [Test]
        public void ScreenNamedCount_IsReported()
        {
            var config = ConfigTestFactory.Create(new List<ScreenDefinition>
            {
                new ScreenDefinition { Name = "Count", IsStart = true },
            });

            Assert.IsNotEmpty(ConfigValidator.Validate(config));
        }

        [Test]
        public void ShowTransitionWithoutTarget_IsReported()
        {
            var config = ConfigTestFactory.Create(
                OneStartScreen(),
                new List<TransitionDefinition>
                {
                    new TransitionDefinition { EventName = "ON_X", Action = NavActionType.Show, TargetScreen = "" },
                });

            Assert.IsNotEmpty(ConfigValidator.Validate(config));
        }

        [Test]
        public void TransitionTargetingUnknownScreen_IsReported()
        {
            var config = ConfigTestFactory.Create(
                OneStartScreen(),
                new List<TransitionDefinition>
                {
                    new TransitionDefinition { EventName = "ON_X", Action = NavActionType.Show, TargetScreen = "Ghost" },
                });

            Assert.IsNotEmpty(ConfigValidator.Validate(config));
        }
    }
}
