namespace HannibalUI.Runtime.Helpers.Observer 
{
    using System;

    public interface IObserver<in TEventArgs> where TEventArgs : EventArgs
    {
        void Notify(Object sender, TEventArgs eventArgs);
    }
}


