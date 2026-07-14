namespace HannibalUI.Tests.Editor
{
    using System.Collections.Generic;
    using HannibalUI.Editor;
    using HannibalUI.Editor.CodeGen;
    using HannibalUI.Runtime.Base;
    using NUnit.Framework;

    public class GeneratorOutputTests
    {
        [Test]
        public void CanvasType_ListsScreensThenCount_ExcludingPopups()
        {
            var config = ConfigTestFactory.Create(new List<ScreenDefinition>
            {
                new ScreenDefinition { Name = "Main", IsStart = true },
                new ScreenDefinition { Name = "Shop" },
                new ScreenDefinition { Name = "Confirm", IsPopup = true },
            });

            var code = EnumGenerator.GenerateCanvasType(config);

            StringAssert.Contains("enum CanvasType", code);
            StringAssert.Contains("Main", code);
            StringAssert.Contains("Shop", code);
            Assert.IsFalse(code.Contains("Confirm"), "Popups must not appear in CanvasType.");
            Assert.Less(code.IndexOf("Shop"), code.IndexOf("Count"), "Count sentinel must be last.");
        }

        [Test]
        public void UIEvents_StartsWithNone_ThenTransitionEvents()
        {
            var config = ConfigTestFactory.Create(
                new List<ScreenDefinition> { new ScreenDefinition { Name = "Main", IsStart = true } },
                new List<TransitionDefinition>
                {
                    new TransitionDefinition { EventName = "ON_SHOP_CLICK", Action = NavActionType.Show, TargetScreen = "Main" },
                });

            var code = EnumGenerator.GenerateUIEvents(config);

            StringAssert.Contains("enum UIEvents", code);
            StringAssert.Contains("ON_SHOP_CLICK", code);
            Assert.Less(code.IndexOf("NONE"), code.IndexOf("ON_SHOP_CLICK"), "NONE must come first.");
        }

        [Test]
        public void Director_HasStartScreen_RoutesAndPopTargetDefault()
        {
            var config = ConfigTestFactory.Create(
                new List<ScreenDefinition>
                {
                    new ScreenDefinition { Name = "Main", IsStart = true },
                    new ScreenDefinition { Name = "Shop" },
                },
                new List<TransitionDefinition>
                {
                    new TransitionDefinition { EventName = "ON_SHOP_CLICK", Action = NavActionType.Push, TargetScreen = "Shop" },
                    new TransitionDefinition { EventName = "ON_BACK_CLICK", Action = NavActionType.Back },
                });

            var code = RouteGenerator.GenerateDirector(config);

            StringAssert.Contains("class VP_GeneratedDirector : VP_Director", code);
            StringAssert.Contains("StartScreen => CanvasType.Main", code);
            StringAssert.Contains("Event = UIEvents.ON_SHOP_CLICK", code);
            StringAssert.Contains("Action = NavActionType.Push", code);
            StringAssert.Contains("Target = CanvasType.Shop", code);
            StringAssert.Contains("Action = NavActionType.Back", code);
            StringAssert.Contains("Target = default", code);
        }

        [Test]
        public void ScreenStub_Canvas_HasScreenTypeAndSetup()
        {
            var screen = new ScreenDefinition { Name = "Main Menu" };

            Assert.AreEqual("VP_MainMenuCanvas", ScreenStubGenerator.ClassName(screen));

            var code = ScreenStubGenerator.Generate(screen);
            StringAssert.Contains(": VP_Canvas", code);
            StringAssert.Contains("ScreenType => CanvasType.MainMenu", code);
            StringAssert.Contains("public override void Setup()", code);
        }

        [Test]
        public void ScreenStub_Popup_ExtendsVpPopup()
        {
            var screen = new ScreenDefinition { Name = "Confirm", IsPopup = true };

            Assert.AreEqual("VP_ConfirmPopup", ScreenStubGenerator.ClassName(screen));
            StringAssert.Contains(": VP_Popup", ScreenStubGenerator.Generate(screen));
        }
    }
}
