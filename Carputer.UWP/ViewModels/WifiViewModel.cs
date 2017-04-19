using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Caliburn.Micro;
using Carputer.UWP.Models;
using PropertyChanged;

namespace Carputer.UWP.ViewModels
{
    [ImplementPropertyChanged]
    public class WifiViewModel : Screen
    {
        public ObservableCollection<WifiNetworkItemViewModel> Networks { get; set; } = new ObservableCollection<WifiNetworkItemViewModel>();

        public WifiViewModel()
        {
            
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

            var access = await WiFiAdapter.RequestAccessAsync();
            if (access == WiFiAccessStatus.Allowed)
            {
                var devices = await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
                if (devices.Count >= 1)
                {
                    var firstAdapter = await WiFiAdapter.FromIdAsync(devices.First().Id);
                    await firstAdapter.ScanAsync();

                    Networks.Clear();
                    var items =
                        firstAdapter.NetworkReport.AvailableNetworks.Select(
                            network => new WifiNetworkItemViewModel(network, firstAdapter)).ToArray();
                    Networks = new ObservableCollection<WifiNetworkItemViewModel>(items);
                }
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }
    }
}
