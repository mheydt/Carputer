using Caliburn.Micro;
using System;

namespace Carputer.UWP.Messages
{
    public class ActivateViewModelMessage
    {
        public Type ViewModelType { get; private set; }

        public ActivateViewModelMessage(Type type)
        {
            ViewModelType = type;
        }
    }

    public class ActivateViewModelMessage<T> : ActivateViewModelMessage where T : Screen
    {

        public ActivateViewModelMessage() : base(typeof(T))
        {
        }
    }
}