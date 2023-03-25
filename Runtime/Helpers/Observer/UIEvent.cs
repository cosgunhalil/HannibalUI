namespace HannibalUI.Runtime.Helpers.Observer 
{
    using System;

    public class UIEvent : EventArgs
    {
        private readonly string _eventName;

        public UIEvent(string eventName) 
        {
            _eventName = eventName;
        }

        private UIEvent() { }

        public string GetEventName() 
        {
            return _eventName;
        }
    }
}

