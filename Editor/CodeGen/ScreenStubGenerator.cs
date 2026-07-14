namespace HannibalUI.Editor.CodeGen
{
    using System.Text;

    /// <summary>
    /// Emits a starter script for each screen: a <c>VP_Canvas</c> subclass for normal screens and
    /// a <c>VP_Popup</c> subclass for popups. These are user-owned — the orchestrator writes them
    /// only when the file doesn't already exist and never overwrites them, so no
    /// <c>&lt;auto-generated&gt;</c> header.
    /// </summary>
    public static class ScreenStubGenerator
    {
        private const string ScreensNamespace = "HannibalUI.Runtime.Screens";

        public static string ClassName(ScreenDefinition screen)
        {
            var pascal = IdentifierUtility.ToPascalCase(screen.Name);
            return screen.IsPopup ? $"VP_{pascal}Popup" : $"VP_{pascal}Canvas";
        }

        public static string FileName(ScreenDefinition screen)
        {
            return ClassName(screen) + ".cs";
        }

        public static string Generate(ScreenDefinition screen)
        {
            return screen.IsPopup ? GeneratePopup(screen) : GenerateCanvas(screen);
        }

        private static string GenerateCanvas(ScreenDefinition screen)
        {
            var pascal = IdentifierUtility.ToPascalCase(screen.Name);
            var className = $"VP_{pascal}Canvas";

            var sb = new StringBuilder();
            sb.Append("namespace ").Append(ScreensNamespace).Append('\n');
            sb.Append("{\n");
            sb.Append("    using HannibalUI.Runtime.Base;\n");
            sb.Append('\n');
            sb.Append("    public class ").Append(className).Append(" : VP_Canvas\n");
            sb.Append("    {\n");
            sb.Append("        public override CanvasType ScreenType => CanvasType.").Append(pascal).Append(";\n");
            sb.Append('\n');
            sb.Append("        public override void Setup()\n");
            sb.Append("        {\n");
            sb.Append("            // TODO: wire up this screen (grab child VP_UIObjects, hook buttons, raise UI events).\n");
            sb.Append("        }\n");
            sb.Append("    }\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        private static string GeneratePopup(ScreenDefinition screen)
        {
            var className = ClassName(screen);

            var sb = new StringBuilder();
            sb.Append("namespace ").Append(ScreensNamespace).Append('\n');
            sb.Append("{\n");
            sb.Append("    using HannibalUI.Runtime.Base;\n");
            sb.Append('\n');
            sb.Append("    public class ").Append(className).Append(" : VP_Popup\n");
            sb.Append("    {\n");
            sb.Append("        // TODO: add this popup's content behaviour.\n");
            sb.Append("    }\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
