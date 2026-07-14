namespace HannibalUI.Runtime.Base
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// Owns screen navigation: a history stack whose top is the active screen, and the animated
    /// transition between screens (via the <see cref="IScreen"/> async seam). Transitions are
    /// serialized through a single CTS, so a new navigation call cancels an in-flight one.
    /// The modal/popup layer is a separate concern (see the popup manager).
    /// </summary>
    public class VP_NavigationService : IDisposable
    {
        private readonly VP_ScreenRegistry _registry;
        private readonly float _transitionDuration;
        private readonly List<CanvasType> _stack = new List<CanvasType>();
        private IScreen _activeScreen;
        private CancellationTokenSource _transitionCts;

        public VP_NavigationService(VP_ScreenRegistry registry, float transitionDuration)
        {
            _registry = registry;
            _transitionDuration = transitionDuration;
        }

        public IScreen ActiveScreen => _activeScreen;
        public int StackCount => _stack.Count;

        /// <summary>Replace the whole history with a single root screen (tab-style switch).</summary>
        public void Show(CanvasType screenType)
        {
            if (!_registry.Contains(screenType))
            {
                Debug.LogError($"VP_NavigationService: cannot Show unregistered screen '{screenType}'.");
                return;
            }

            _stack.Clear();
            _stack.Add(screenType);
            TransitionTo(screenType);
        }

        /// <summary>Overlay a new screen on top, keeping the current one in history.</summary>
        public void Push(CanvasType screenType)
        {
            if (!_registry.Contains(screenType))
            {
                Debug.LogError($"VP_NavigationService: cannot Push unregistered screen '{screenType}'.");
                return;
            }

            if (_stack.Count > 0 && _stack[_stack.Count - 1] == screenType)
            {
                return;
            }

            _stack.Add(screenType);
            TransitionTo(screenType);
        }

        /// <summary>Return to the previous screen in history. No-op at the root.</summary>
        public void Pop()
        {
            if (_stack.Count <= 1)
            {
                return;
            }

            _stack.RemoveAt(_stack.Count - 1);
            TransitionTo(_stack[_stack.Count - 1]);
        }

        public void Back()
        {
            Pop();
        }

        /// <summary>Pop until <paramref name="screenType"/> is on top. Must already be in the stack.</summary>
        public void PopTo(CanvasType screenType)
        {
            int index = _stack.LastIndexOf(screenType);
            if (index < 0)
            {
                Debug.LogError($"VP_NavigationService: cannot PopTo '{screenType}' — it is not in the stack.");
                return;
            }

            _stack.RemoveRange(index + 1, _stack.Count - index - 1);
            TransitionTo(screenType);
        }

        public void Dispose()
        {
            _transitionCts?.Cancel();
            _transitionCts?.Dispose();
            _transitionCts = null;
        }

        private void TransitionTo(CanvasType screenType)
        {
            var target = _registry.Get(screenType);
            if (target == null || ReferenceEquals(_activeScreen, target))
            {
                return;
            }

            _transitionCts?.Cancel();
            _transitionCts?.Dispose();
            _transitionCts = new CancellationTokenSource();
            SwitchAsync(target, _transitionCts.Token);
        }

        private async UniTaskVoid SwitchAsync(IScreen target, CancellationToken cancellationToken)
        {
            try
            {
                if (_activeScreen != null)
                {
                    await _activeScreen.DeactivateAsync(_transitionDuration, cancellationToken);
                }

                _activeScreen = target;
                await target.ActivateAsync(_transitionDuration, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Superseded by a newer navigation call; that call now owns the active screen.
            }
        }
    }
}
