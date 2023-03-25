
namespace com.voxelpixel.hannibal_ui.sample
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using HannibalUI.Runtime.Base;


    public class SampleApp : MonoBehaviour
    {
        private VP_Director director;
        // Start is called before the first frame update
        private void Start()
        {
            director = GameObject.FindObjectOfType<VP_Director>();
#if UNITY_EDITOR
            if (director == null) 
            {
                Debug.LogError("There is no LB_UIManager in the scene. Please add one!");
            }
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                director.EnableCanvas(CanvasType.Main);
            }
            else if (Input.GetKeyDown(KeyCode.W)) 
            {
                director.EnableCanvas(CanvasType.Market);
            }
            else if (Input.GetKeyDown(KeyCode.E)) 
            {
                director.EnableCanvas(CanvasType.Characters);
            }
        }
    }
}


