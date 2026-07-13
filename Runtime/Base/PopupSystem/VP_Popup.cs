namespace HannibalUI.Runtime.Base
{
    using UnityEngine;

    public class VP_Popup : MonoBehaviour
    {
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
