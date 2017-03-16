using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
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
        private IPowerShutdownMonitorService _powerMonitorService;
        private IContinuousSpeechRecognizer _continousSpeechRecognizer;

        public BootstrapService(
            IEventAggregator eventAggregator, 
            ICacheService cacheService, 
            ISettingsService settingService, 
            IGPSService gpsService,
            IPowerShutdownMonitorService powerMonitorService,
            IContinuousSpeechRecognizer continuousSpeechRecognizer)
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingService;
            _gpsService = gpsService;
            _cacheService = cacheService;
            _powerMonitorService = powerMonitorService;
            _continousSpeechRecognizer = continuousSpeechRecognizer;

            _powerMonitorService.DoShutdown += async (s, e) =>
            {
                await ShutdownAsync();
                ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.Zero);
            };
/*
            _services = (new object[]
            {
                cacheService,
                settingService,
                gpsService,
                _powerMonitorService
            }).OfType<IService>().ToArray();
            */

            CacheService.ApplicationName = "Carputer";
        }

        public async Task InitializeAsync()
        {
            (_cacheService as IService).StartAsync();
            (_settingsService as IService).StartAsync();
            (_gpsService as IService).StartAsync();
            (_powerMonitorService as IService).StartAsync();
            (_continousSpeechRecognizer as IService).StartAsync();
            //var tasks = _services.Select(s => s.StartAsync());
            //await Task.WhenAll(tasks);

            await Task.CompletedTask;
        }

        public async Task ShutdownAsync()
        {
            await (_powerMonitorService as IService).StopAsync();
            await (_gpsService as IService).StopAsync();
            await (_settingsService as IService).StopAsync();
            await (_cacheService as IService).StopAsync();
        }
    }
}
