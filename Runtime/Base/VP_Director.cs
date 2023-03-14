//TODO: generate automatically! 
namespace com.voxelpixel.hannibal_ui.base_component
{
    using System.Collections;
    using UnityEngine;

    public class VP_Director : MonoBehaviour
    {
        private const float CANVAS_ACTIVATION_TIME = .5f;
        [SerializeField]private VP_Canvas[] canvases;//TODO: solve the order issue! Order is really important in here! 
        private VP_Canvas activeCanvas = null;

        public void Awake()
        {
            foreach (var canvas in canvases)
            {
                canvas.PreInit();
            }
        }

        public IEnumerator Start()
        {
            foreach (var canvas in canvases)
            {
                canvas.Init();
                canvas.LateInit();
                canvas.Deactivate(0);
            }

            //TODO: Solve this work-around!
            yield return new WaitForSeconds(0.01f);

            activeCanvas = canvases[(int)CanvasType.Main];
            activeCanvas.Activate(0);
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
    }
}