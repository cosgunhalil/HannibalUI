//TODO: generate automatically! 
namespace com.voxelpixel.hannibal_ui.base_component
{
    using System.Collections;
    using UnityEngine;

    public class LB_UIManager : MonoBehaviour
    {
        private VP_Canvas[] canvases;

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
            //TODO: handle
        }

        private IEnumerator EnableRequestedCanvas(VP_Canvas targetCanvas)
        {
            //TODO: handle
            yield break;
        }
    }
}