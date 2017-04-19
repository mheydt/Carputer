using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Carputer.Phone.UWP.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private IEventAggregator _eventAggregator;
        private WinRTContainer _container;

        public ShellViewModel()
        {
            
        }

        public ShellViewModel(WinRTContainer container, IEventAggregator eventAggregator)
        {
            _container = container;
            _eventAggregator = eventAggregator;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            navigate(typeof(InitializingViewModel));
        }

        private void navigate(Type viewModelType)
        {
            var vm = _container.GetAllInstances(viewModelType).First();
            ActivateItem(vm);
        }
    }
}
