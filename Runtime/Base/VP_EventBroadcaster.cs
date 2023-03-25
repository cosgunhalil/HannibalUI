namespace HannibalUI.Runtime.Base
{
    using HannibalUI.Runtime.Helpers.Observer;
    using System.Collections.Generic;
    using UnityEngine;

    public class VP_EventBroadcaster : ISubject<VP_UIEvent>
    {
        private List<IObserver<VP_UIEvent>> _observers;

        public VP_EventBroadcaster()
        {
            _observers = new List<IObserver<VP_UIEvent>>();
        }

        public void Register(IObserver<VP_UIEvent> observer)
        {
            if (_observers.Contains(observer))
            {
                Debug.LogWarning("This observer has already been added to the list!");
                return;
            }

            _observers.Add(observer);
        }

        public void UnRegister(IObserver<VP_UIEvent> observer)
        {
            if (!_observers.Contains(observer))
            {
                Debug.LogWarning("The observer you trying to unregister is already unregistered!");
                return;
            }

            _observers.Remove(observer);
        }

        public void BroadcastEvent(VP_UIEvent eventArgs)
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                _observers[i].Notify(this, eventArgs);
            }
        }
    }
}
