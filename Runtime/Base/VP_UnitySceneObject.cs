namespace HannibalUI.Runtime.Base
{
    using UnityEngine;

    public class VP_UnitySceneObject : MonoBehaviour
    {
        public virtual void PreInit() { }
        public virtual void Init() { }
        public virtual void LateInit() { }
    }
}