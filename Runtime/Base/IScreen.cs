namespace HannibalUI.Runtime.Base
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// The view-backend-agnostic contract the director/navigation talks to. Unity UI (uGUI)
    /// screens implement it via <see cref="VP_Canvas"/>; a UI Toolkit backend can implement the
    /// same seam later without touching the navigation core.
    /// </summary>
    public interface IScreen
    {
        /// <summary>Identity used by the screen registry instead of array position.</summary>
        CanvasType ScreenType { get; }

        /// <summary>Raised when something on the screen requests a UI action (e.g. a nav button).</summary>
        event Action<UIEvents> UIEventRaised;

        void PreInit();
        void Init();
        void LateInit();
        void TearDown();

        /// <summary>Show the screen; the task completes when the activation transition finishes.</summary>
        UniTask ActivateAsync(float duration, CancellationToken cancellationToken);

        /// <summary>Hide the screen; the task completes when the deactivation transition finishes.</summary>
        UniTask DeactivateAsync(float duration, CancellationToken cancellationToken);
    }
}
