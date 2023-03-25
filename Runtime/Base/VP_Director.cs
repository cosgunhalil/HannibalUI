//TODO: generate automatically! 
namespace com.voxelpixel.hannibal_ui.base_component
{
    using HannibalUI.Runtime.Helpers.Observer;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class VP_Director : MonoBehaviour, ISubject<UIEvent>
    {
        private const float CANVAS_ACTIVATION_TIME = .5f;
        [SerializeField]private VP_Canvas[] canvases;//TODO: solve the order issue! Order is really important in here! 
        private VP_Canvas activeCanvas = null;
        private List<IObserver<UIEvent>> _observers;

        public void Awake()
        {

            foreach (var canvas in canvases)
            {
                canvas.PreInit();
            }
        }

        public void Start()
        {
            foreach (var canvas in canvases)
            {
                canvas.Init();
            }

            foreach (var canvas in canvases)
            {
                canvas.LateInit();
            }

            EnableCanvas(CanvasType.Main);
        }

        public void OnDestroy()
        {
            foreach (var canvas in canvases)
            {
                canvas.OnDestroyCalled();
            }
        }

        public void EnableCanvas(CanvasType canvasType)
        {
            if (canvases.Length == 0)
            {
                return;
            }

            var targetCanvas = canvases[(int)canvasType];

            if (activeCanvas == null)
            {
                activeCanvas = targetCanvas;
                activeCanvas.Activate(CANVAS_ACTIVATION_TIME);
                return;
            }

            if (activeCanvas == targetCanvas) 
            {
                return;
            }

            //TODO: use Unitask instead of coroutine!
            StopCoroutine("EnableRequestedCanvas");
            StartCoroutine("EnableRequestedCanvas", targetCanvas);
        }

        private IEnumerator EnableRequestedCanvas(VP_Canvas targetCanvas)
        {
            if (activeCanvas != null)
            {
                activeCanvas.Deactivate(CANVAS_ACTIVATION_TIME);
            }

            yield return new WaitForSeconds(CANVAS_ACTIVATION_TIME);//TODO: We hate magic numbers!!!

            activeCanvas = targetCanvas;
            activeCanvas.Activate(CANVAS_ACTIVATION_TIME);
        }

        public void Register(IObserver<UIEvent> observer)
        {
            if (_observers.Contains(observer)) 
            {
                Debug.LogWarning("This observer is already in the list!");
                return;
            }

            _observers.Add(observer);
        }

        public void UnRegister(IObserver<UIEvent> observer)
        {
            if (!_observers.Contains(observer)) 
            {
                Debug.LogWarning("The observer you trying to unregister is already unregistered!");
                return;
            }

            _observers.Remove(observer);
        }

        public void BroadcastEvent(UIEvent eventArgs)
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                _observers[i].Notify(this, eventArgs);
            }
        }
    }
}