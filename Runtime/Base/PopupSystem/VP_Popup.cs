namespace HannibalUI.Runtime.Base 
{
    using HannibalUI.Runtime.Helpers.Memory;
    using UnityEngine;

    public class VP_Popup : MonoBehaviour, IPoolable
    {
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Initialize()
        {

        }
    }
}

