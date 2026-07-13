namespace HannibalUI.Runtime.UIElements
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using HannibalUI.Runtime.Base;

    [RequireComponent(typeof(Button))]
    public class VP_Button : VP_UIObject
    {
        public event Action OnPointerDownEvent;
        public event Action OnPointerUpEvent;
        public event Action OnPointerEnterEvent;
        public event Action OnPointerExitEvent;

        protected Button _button;
        private EventTrigger _eventTrigger;

        public override void Init()
        {
            base.Init();
            _button = GetComponent<Button>();
            SetEventTrigger();
            SetupButtonActions();
        }

        private void SetEventTrigger()
        {
            _eventTrigger = gameObject.GetComponent<EventTrigger>();
            if (_eventTrigger == null)
            {
                _eventTrigger = gameObject.AddComponent<EventTrigger>();
            }
        }

        private void SetupButtonActions()
        {
            AddActionToButton(EventTriggerType.PointerEnter, OnPointerEnter);
            AddActionToButton(EventTriggerType.PointerExit, OnPointerExit);
            AddActionToButton(EventTriggerType.PointerDown, OnPointerDown);
            AddActionToButton(EventTriggerType.PointerUp, OnPointerUp);
        }

        protected virtual void OnPointerDown()
        {
            OnPointerDownEvent?.Invoke();
        }

        protected virtual void OnPointerUp()
        {
            OnPointerUpEvent?.Invoke();
        }

        protected virtual void OnPointerEnter()
        {
            OnPointerEnterEvent?.Invoke();
        }

        protected virtual void OnPointerExit()
        {
            OnPointerExitEvent?.Invoke();
        }

        private void AddActionToButton(EventTriggerType eventTriggerType, Action action)
        {
            var entry = new EventTrigger.Entry
            {
                eventID = eventTriggerType
            };
            entry.callback.AddListener((e) => { action(); });
            _eventTrigger.triggers.Add(entry);
        }
    }
}
