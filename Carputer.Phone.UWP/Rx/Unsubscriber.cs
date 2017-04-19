using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carputer.Phone.UWP.Models;

namespace Carputer.Phone.UWP.Rx
{
    public class Unsubscriber<T> : IDisposable
    {
        private IObserver<T> _observer;
        private List<IObserver<T>> _observers;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observer = observer;
            _observers = observers;
        }

        public void Dispose()
        {
            if (_observer != null) _observers.Remove(_observer);
        }
    }
}
