namespace HannibalUI.Editor.CodeGen
{
    using System.Collections.Generic;
    using HannibalUI.Runtime.Base;

    /// <summary>
    /// Validates a <see cref="UIProjectConfig"/> before generation. Returns a list of error
    /// messages; an empty list means the config is safe to generate from.
    /// </summary>
    public static class ConfigValidator
    {
        public static List<string> Validate(UIProjectConfig config)
        {
            var errors = new List<string>();

            if (config == null)
            {
                errors.Add("Config is null.");
                return errors;
            }

            var screenMembers = new HashSet<string>();
            int nonPopupCount = 0;
            int startCount = 0;

            foreach (var screen in config.Screens)
            {
                if (string.IsNullOrWhiteSpace(screen.Name))
                {
                    errors.Add("A screen has an empty name.");
                    continue;
                }

                var member = IdentifierUtility.ToPascalCase(screen.Name);

                if (member == "_")
                {
                    errors.Add($"Screen '{screen.Name}' does not produce a valid identifier.");
                    continue;
                }

                if (screen.IsPopup)
                {
                    continue; // popups aren't in CanvasType and don't have a start flag.
                }

                nonPopupCount++;

                if (member == "Count")
                {
                    errors.Add($"Screen '{screen.Name}' collides with the reserved CanvasType sentinel 'Count'.");
                }

                if (!screenMembers.Add(member))
                {
                    errors.Add($"Duplicate screen '{member}' (from '{screen.Name}').");
                }

                if (screen.IsStart)
                {
                    startCount++;
                }
            }

            if (nonPopupCount == 0)
            {
                errors.Add("At least one non-popup screen is required.");
            }
            else if (startCount == 0)
            {
                errors.Add("No start screen: mark exactly one screen as IsStart.");
            }
            else if (startCount > 1)
            {
                errors.Add($"{startCount} start screens marked; exactly one is allowed.");
            }

            var eventMembers = new HashSet<string>();

            foreach (var transition in config.Transitions)
            {
                if (string.IsNullOrWhiteSpace(transition.EventName))
                {
                    errors.Add("A transition has an empty event name.");
                    continue;
                }

                var eventMember = IdentifierUtility.ToValidIdentifier(transition.EventName);

                if (eventMember == "NONE")
                {
                    errors.Add($"Transition event '{transition.EventName}' collides with the reserved 'NONE'.");
                }

                if (!eventMembers.Add(eventMember))
                {
                    errors.Add($"Duplicate transition event '{eventMember}' (from '{transition.EventName}').");
                }

                bool needsTarget = transition.Action == NavActionType.Show || transition.Action == NavActionType.Push;

                if (!needsTarget)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(transition.TargetScreen))
                {
                    errors.Add($"Transition '{transition.EventName}' ({transition.Action}) needs a target screen.");
                    continue;
                }

                var target = IdentifierUtility.ToPascalCase(transition.TargetScreen);

                if (!screenMembers.Contains(target))
                {
                    errors.Add($"Transition '{transition.EventName}' targets unknown screen '{transition.TargetScreen}'.");
                }
            }

            return errors;
        }
    }
}
