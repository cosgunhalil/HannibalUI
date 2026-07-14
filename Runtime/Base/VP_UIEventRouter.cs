namespace HannibalUI.Runtime.Base
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum NavActionType
    {
        Show,
        Push,
        Pop,
        Back
    }

    /// <summary>
    /// A single mapping from a <see cref="UIEvents"/> value to a navigation action. Authored in
    /// the Inspector today; the plan is to generate these from the UI config in Phase 2.
    /// <see cref="Target"/> is used by <see cref="NavActionType.Show"/>/<see cref="NavActionType.Push"/>
    /// and ignored by Pop/Back.
    /// </summary>
    [Serializable]
    public struct NavRoute
    {
        public UIEvents Event;
        public NavActionType Action;
        public CanvasType Target;
    }

    /// <summary>
    /// Turns raised <see cref="UIEvents"/> into navigation calls using a data-driven route table
    /// instead of a hardcoded switch. The per-event mapping is data; only the fixed set of action
    /// kinds (Show/Push/Pop/Back) is code.
    /// </summary>
    public class VP_UIEventRouter
    {
        private readonly VP_NavigationService _navigation;
        private readonly Dictionary<UIEvents, NavRoute> _routes = new Dictionary<UIEvents, NavRoute>();

        public VP_UIEventRouter(VP_NavigationService navigation, IReadOnlyList<NavRoute> routes)
        {
            _navigation = navigation;

            if (routes == null)
            {
                return;
            }

            for (int i = 0; i < routes.Count; i++)
            {
                var route = routes[i];

                if (_routes.ContainsKey(route.Event))
                {
                    Debug.LogError($"VP_UIEventRouter: duplicate route for '{route.Event}'; keeping the first.");
                    continue;
                }

                _routes.Add(route.Event, route);
            }
        }

        public void Handle(UIEvents uiEvent)
        {
            if (!_routes.TryGetValue(uiEvent, out var route))
            {
                return;
            }

            switch (route.Action)
            {
                case NavActionType.Show:
                    _navigation.Show(route.Target);
                    break;
                case NavActionType.Push:
                    _navigation.Push(route.Target);
                    break;
                case NavActionType.Pop:
                    _navigation.Pop();
                    break;
                case NavActionType.Back:
                    _navigation.Back();
                    break;
                default:
                    Debug.LogError($"VP_UIEventRouter: unhandled action '{route.Action}'.");
                    break;
            }
        }
    }
}
