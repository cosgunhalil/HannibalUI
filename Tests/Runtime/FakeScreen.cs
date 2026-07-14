namespace HannibalUI.Tests.Runtime
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using HannibalUI.Runtime.Base;

    /// <summary>
    /// A lightweight <see cref="IScreen"/> for tests: records activate/deactivate calls and
    /// completes its transitions instantly so navigation can be asserted deterministically.
    /// </summary>
    public class FakeScreen : IScreen
    {
        private readonly CanvasType _type;

        public FakeScreen(CanvasType type)
        {
            _type = type;
        }

        public CanvasType ScreenType => _type;

        public int ActivateCount { get; private set; }
        public int DeactivateCount { get; private set; }

#pragma warning disable 0067
        public event Action<UIEvents> UIEventRaised;
#pragma warning restore 0067

        public void PreInit() { }
        public void Init() { }
        public void LateInit() { }
        public void TearDown() { }

        public UniTask ActivateAsync(float duration, CancellationToken cancellationToken)
        {
            ActivateCount++;
            return UniTask.CompletedTask;
        }

        public UniTask DeactivateAsync(float duration, CancellationToken cancellationToken)
        {
            DeactivateCount++;
            return UniTask.CompletedTask;
        }
    }
}
