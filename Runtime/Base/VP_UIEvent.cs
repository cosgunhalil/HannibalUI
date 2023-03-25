namespace HannibalUI.Runtime.Base 
{
    using System;

    public class VP_UIEvent : EventArgs
    {
        private readonly UIEvents _event;

        public VP_UIEvent(UIEvents uiEvent) 
        {
            _event = uiEvent;
        }

        private VP_UIEvent() { }

        public UIEvents GetEventType() 
        {
            return _event;
        }
    }
}

