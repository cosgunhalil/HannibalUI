namespace com.voxelpixel.hannibal_ui.sample 
{
    using com.voxelpixel.hannibal_ui.base_component;
    using UnityEngine;

    public class CommonsCanvas : VP_Canvas
    {
        [SerializeField] private VP_Button _mainMenuButton;
        [SerializeField] private VP_Button _charactersButton;
        [SerializeField] private VP_Button _marketButton;
        [SerializeField] private VP_HorizontalRectTransformContainer _horizontalRectTransformContainer;

        public override void Setup()
        {
            Debug.Log("CommonsCanvas::Setup()");
            _mainMenuButton.OnPointerDownEvent += MainMenuButton_OnPointerDownEvent;
            _charactersButton.OnPointerDownEvent += CharactersButton_OnPointerDownEvent;
            _marketButton.OnPointerDownEvent += MarketButton_OnPointerDownEvent;
        }

        private void MainMenuButton_OnPointerDownEvent()
        {
            Debug.Log("CommonsCanvas::MainMenuButton_OnPointerDownEvent()");
        }

        private void CharactersButton_OnPointerDownEvent()
        {
            Debug.Log("CommonsCanvas::CharactersButton_OnPointerDownEvent()");
        }

        private void MarketButton_OnPointerDownEvent()
        {
            Debug.Log("CommonsCanvas::MarketButton_OnPointerDownEvent()");
        }

        public override void LateInit()
        {
            if (_horizontalRectTransformContainer == null) 
            {
#if UNITY_EDITOR
                Debug.LogError("_horizontalRectTransformContainer is null!");
#endif
                return;
            }

            _horizontalRectTransformContainer.Configure(new Vector2(canvasSize.x, canvasSize.y * .25f), 
                new Padding() { Top = 0f, Bottom = 0f, Left = 1.0f, Right = 1.0f }, 
                5);
        }
    }
}
