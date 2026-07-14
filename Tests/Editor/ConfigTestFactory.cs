namespace HannibalUI.Tests.Editor
{
    using System.Collections.Generic;
    using System.Reflection;
    using HannibalUI.Editor;
    using UnityEngine;

    /// <summary>
    /// Builds a UIProjectConfig for tests by assigning its private serialized lists via reflection,
    /// so the production config keeps a read-only API.
    /// </summary>
    public static class ConfigTestFactory
    {
        public static UIProjectConfig Create(
            List<ScreenDefinition> screens, List<TransitionDefinition> transitions = null)
        {
            var config = ScriptableObject.CreateInstance<UIProjectConfig>();
            SetField(config, "_screens", screens ?? new List<ScreenDefinition>());
            SetField(config, "_transitions", transitions ?? new List<TransitionDefinition>());
            return config;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var field = typeof(UIProjectConfig).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }
    }
}
