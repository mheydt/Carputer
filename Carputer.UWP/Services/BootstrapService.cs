using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carputer.UWP.Interfaces;

namespace Carputer.UWP.Services
{
    public interface IBootstrapService
    {
        Task InitializeAsync();
        Task ShutdownAsync();
    }

    public class BootstrapService : IBootstrapService
    {
        private ISettingsService _settingsService;
        private IGPSService _gpsService;
        private IEventAggregator _eventAggregator;
        private ICacheService _cacheService;

        private IService[] _services;

        public BootstrapService(IEventAggregator eventAggregator, ICacheService cacheService, ISettingsService settingService, IGPSService gpsService)
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingService;
            _gpsService = gpsService;
            _cacheService = cacheService;

            _services = (new object[]
            {
                cacheService,
                settingService,
                gpsService
            }).OfType<IService>().ToArray();

            CacheService.ApplicationName = "Carputer";
        }

        public async Task InitializeAsync()
        {
            await _services[0].StartAsync();
            await _services[1].StartAsync();
            await _services[2].StartAsync();
            //var tasks = _services.Select(s => s.StartAsync());
            //await Task.WhenAll(tasks);
        }

        public async Task ShutdownAsync()
        {
            //var tasks = _services.Select(s => s.StopAsync());
            //await Task.WhenAll(tasks);
            await _services[2].StopAsync();
            await _services[1].StopAsync();
            await _services[0].StopAsync();
        }
    }
}
