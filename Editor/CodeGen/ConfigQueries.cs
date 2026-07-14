namespace HannibalUI.Editor.CodeGen
{
    using System.Collections.Generic;

    /// <summary>Pure read helpers over a <see cref="UIProjectConfig"/> (unit-testable).</summary>
    public static class ConfigQueries
    {
        /// <summary>Names of the non-popup screens with a non-empty name, in config order.</summary>
        public static List<string> NonPopupScreenNames(UIProjectConfig config)
        {
            var names = new List<string>();

            if (config == null)
            {
                return names;
            }

            foreach (var screen in config.Screens)
            {
                if (!screen.IsPopup && !string.IsNullOrWhiteSpace(screen.Name))
                {
                    names.Add(screen.Name);
                }
            }

            return names;
        }
    }
}
