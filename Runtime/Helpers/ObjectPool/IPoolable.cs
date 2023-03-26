namespace HannibalUI.Runtime.Helpers.Memory 
{
    public interface IPoolable
    {
        void Initialize();
        void Activate();
        void Deactivate();
    }
}

