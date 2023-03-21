namespace com.voxelpixel.hannibal_ui.base_component 
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(RectTransform))]
    public class VP_HorizontalRectTransformContainer : VP_UIObject
    {
        [SerializeField]
        private RectTransform[] _rectTransforms;
        private Padding _padding;
        private float _spacing;

        public void Configure(Vector2 size, Padding padding, float spacing) 
        {
            ObjectRectTransform.sizeDelta = size;
            _padding = padding;
            _spacing = spacing;
            AllignChildren();
        }

        private void AllignChildren()
        {
            var childRectWidth = (ObjectRectTransform.sizeDelta.x - (_padding.Left + _padding.Right + (_spacing * (_rectTransforms.Length - 1)))) / _rectTransforms.Length;
            foreach (var rect in _rectTransforms)
            {
                rect.sizeDelta = new Vector2(childRectWidth, ObjectRectTransform.sizeDelta.y - (_padding.Bottom + _padding.Top));
            }
        }
    }

    public struct Padding 
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
    }
}

