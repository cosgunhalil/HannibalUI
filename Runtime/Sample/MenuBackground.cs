namespace com.voxelpixel.hannibal_ui.sample 
{
    using com.voxelpixel.hannibal_ui.animation;
    using com.voxelpixel.hannibal_ui.base_component;
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
                ActivatedCoordinate = ObjectRectTransform.anchoredPosition,
                DeactivatedCoordinate = new Vector2(
                canvasSize.x * .5f + ObjectRectTransform.sizeDelta.x * .5f,
                ObjectRectTransform.anchoredPosition.y)
            };

            AnimationComponents.Add(moveAnimationComponent);
            ObjectRectTransform.anchoredPosition = moveAnimationComponent.DeactivatedCoordinate;
        }
    }
}

