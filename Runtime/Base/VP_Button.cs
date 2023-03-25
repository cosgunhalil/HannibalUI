namespace HannibalUI.Runtime.Base 
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using DG.Tweening;

    [RequireComponent(typeof(Button))]
    public class VP_Button : VP_UIObject
    {
        public delegate void MessageAction();
        public event MessageAction OnPointerDownEvent;
        public event MessageAction OnPointerUpEvent;
        public event MessageAction OnPointerEnterEvent;
        public event MessageAction OnPointerExitEvent;

        protected float _localScaleTemp;
        protected Button _button;
        private EventTrigger _eventTrigger;
        private Transform _transform;

        public override void Init()
        {
            base.Init();
            _button = GetComponent<Button>();
            _localScaleTemp = transform.localScale.x;
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

        protected void OnPointerDown()
        {
            OnPointerDownEvent?.Invoke();
        }

        protected void OnPointerUp()
        {
            OnPointerUpEvent?.Invoke();
        }

        protected void OnPointerEnter()
        {
            OnPointerEnterEvent?.Invoke();
        }

        protected void OnPointerExit()
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

