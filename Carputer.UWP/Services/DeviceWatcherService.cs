using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carputer.UWP.Interfaces;
using Windows.Devices.Enumeration;

namespace Carputer.UWP.Services
{
    public interface IDeviceWatcherService
    {
        
    }

    public class DeviceWatcherService : IDeviceWatcherService, IService
    {
        private DeviceWatcher _watcher;

        public DeviceWatcherService()
        {
            _watcher = DeviceInformation.CreateWatcher();
            _watcher.Added += _watcher_Added;
            _watcher.Removed += _watcher_Removed;
        }

        private void _watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private void _watcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
        }

        public async Task StartAsync()
        {
            _watcher.Start();
        }

        public async Task StopAsync()
        {
            _watcher.Stop();
        }
    }
}
