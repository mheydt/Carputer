using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Carputer.Phone.UWP.Interfaces;
using Carputer.Phone.UWP.Rx;
using Carputer.Phone.UWP.Services;

namespace Carputer.Phone.UWP.Models
{
    public interface IAppModel
    {
        Task InitializeAsync();
    }


    public class AppModel : IAppModel, INeedInitialization, INeedShutdown, IObservable<AppModel.AppModelMessage>
    {
        public class AppModelMessage
        {
        }

        private static List<IObserver<AppModelMessage>> _observers = new List<IObserver<AppModelMessage>>();
        private ICarputerProxyService _carputerProxyService;

        public AppModel(ICarputerProxyService carputerProxyService)
        {
            _carputerProxyService = carputerProxyService;
        }

        public async Task InitializeAsync()
        {
            await BackgroundExecutionManager.RequestAccessAsync();

            await initializeServiceAsync(_carputerProxyService);
        }

        private async Task initializeServiceAsync(IService service)
        {
            await service.InitializeAsync();
        }

        public IDisposable Subscribe(IObserver<AppModel.AppModelMessage> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
            return new Unsubscriber<AppModelMessage>(_observers, observer);
        }

        public async Task ShutdownAsync()
        {
            await Task.CompletedTask;
        }
    }
}
