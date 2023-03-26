namespace com.voxelpixel.hannibal_ui.sample 
{
    using com.voxelpixel.hannibal_ui.animation;
    using HannibalUI.Runtime.Base;
    using DG.Tweening;
    using UnityEngine;

    public class MenuBackground : VP_UIObject
    {
        public override void Setup(Vector2 canvasSize)
        {
            ObjectRectTransform.sizeDelta = canvasSize;
            CreateMoveAnimationComponent(canvasSize);
        }

        private void CreateMoveAnimationComponent(Vector2 canvasSize)
        {
            MoveAnimationComponent moveAnimationComponent = new(ObjectRectTransform)
            {
                AnimationEase = Ease.InOutSine,
                ActivatedCoordinate = new Vector2(0, 0),
                DeactivatedCoordinate = new Vector2(-canvasSize.x, 0)
            };

            AnimationComponents.Add(moveAnimationComponent);
            ObjectRectTransform.anchoredPosition = moveAnimationComponent.DeactivatedCoordinate;
        }
    }
}

