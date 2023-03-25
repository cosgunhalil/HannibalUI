namespace HannibalUI.Runtime.Helpers.Observer 
{
    using System;

    public interface IObserver<in T> where T : EventArgs
    {
        void Notify(Object sender, T e);
    }
}


