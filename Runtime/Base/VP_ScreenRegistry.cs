namespace HannibalUI.Runtime.Base
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Resolves screens by their declared <see cref="CanvasType"/> instead of by array position,
    /// so the order of the serialized screen list no longer matters. Built once at init from the
    /// available <see cref="IScreen"/>s.
    /// </summary>
    public class VP_ScreenRegistry
    {
        private readonly Dictionary<CanvasType, IScreen> _screens = new Dictionary<CanvasType, IScreen>();

        public VP_ScreenRegistry(IReadOnlyList<IScreen> screens)
        {
            for (int i = 0; i < screens.Count; i++)
            {
                var screen = screens[i];

                if (screen == null)
                {
                    Debug.LogError($"VP_ScreenRegistry: screen at index {i} is null and was skipped.");
                    continue;
                }

                if (_screens.ContainsKey(screen.ScreenType))
                {
                    Debug.LogError($"VP_ScreenRegistry: duplicate screen for '{screen.ScreenType}'; keeping the first and skipping the rest.");
                    continue;
                }

                _screens.Add(screen.ScreenType, screen);
            }
        }

        public bool Contains(CanvasType screenType)
        {
            return _screens.ContainsKey(screenType);
        }

        public bool TryGet(CanvasType screenType, out IScreen screen)
        {
            return _screens.TryGetValue(screenType, out screen);
        }

        public IScreen Get(CanvasType screenType)
        {
            if (_screens.TryGetValue(screenType, out var screen))
            {
                return screen;
            }

            Debug.LogError($"VP_ScreenRegistry: no screen registered for '{screenType}'.");
            return null;
        }
    }
}
