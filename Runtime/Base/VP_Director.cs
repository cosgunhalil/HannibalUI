//TODO: generate automatically! 
namespace com.voxelpixel.hannibal_ui.base_component
{
    using System.Collections;
    using UnityEngine;

    public class VP_Director : MonoBehaviour
    {
        [SerializeField]
        private VP_Canvas[] canvases;
        private VP_Canvas activeCanvas = null;

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
                canvas.LateInit();
            }
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
                activeCanvas.Activate();
                return;
            }

            if (activeCanvas == targetCanvas) 
            {
                return;
            }

            StopCoroutine("EnableRequestedCanvas");
            StartCoroutine("EnableRequestedCanvas", targetCanvas);
        }

        private IEnumerator EnableRequestedCanvas(VP_Canvas targetCanvas)
        {
            if (activeCanvas != null)
            {
                activeCanvas.Deactivate();
            }

            yield return new WaitForSeconds(.5f);

            activeCanvas = targetCanvas;
            activeCanvas.Activate();
        }
    }
}