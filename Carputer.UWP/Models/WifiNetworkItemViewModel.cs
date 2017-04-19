using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using PropertyChanged;
using Windows.UI.Xaml.Media.Imaging;

namespace Carputer.UWP.Models
{
    [ImplementPropertyChanged]
    public class WifiNetworkItemViewModel
    {
        public WiFiAvailableNetwork AvailableNetwork { get; private set; }
        public String Ssid { get { return AvailableNetwork.Ssid; } }
        public String Bssid { get { return AvailableNetwork.Bssid; } }

        public String ChannelCenterFrequency => string.Format("{0}kHz", AvailableNetwork.ChannelCenterFrequencyInKilohertz);
        public String Rssi => string.Format("{0}dBm", AvailableNetwork.NetworkRssiInDecibelMilliwatts);
        public String SecuritySettings => string.Format("Authentication: {0}; Encryption: {1}", AvailableNetwork.SecuritySettings.NetworkAuthenticationType, AvailableNetwork.SecuritySettings.NetworkEncryptionType);

        public String ConnectivityLevel { get; private set; }
        public BitmapImage WiFiImage { get; private set; }

        private WiFiAdapter _adapter;

        public WifiNetworkItemViewModel(WiFiAvailableNetwork availableNetwork, WiFiAdapter adapter)
        {
            AvailableNetwork = availableNetwork;
            _adapter = adapter;
            UpdateWiFiImage();
            UpdateConnectivityLevel();
        }

        private void UpdateWiFiImage()
        {
            var imageFileNamePrefix = "secure";
            if (AvailableNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211)
            {
                imageFileNamePrefix = "open";
            }

            var imageFileName = string.Format("ms-appx:/Assets/wifi/{0}_{1}bar.png", imageFileNamePrefix, AvailableNetwork.SignalBars);

            WiFiImage = new BitmapImage(new Uri(imageFileName));
        }

        public async void UpdateConnectivityLevel()
        {
            var connectivityLevel = "Not Connected";
            string connectedSsid = null;

            var connectedProfile = await _adapter.NetworkAdapter.GetConnectedProfileAsync();
            if (connectedProfile != null &&
                connectedProfile.IsWlanConnectionProfile &&
                connectedProfile.WlanConnectionProfileDetails != null)
            {
                connectedSsid = connectedProfile.WlanConnectionProfileDetails.GetConnectedSsid();
            }

            if (!string.IsNullOrEmpty(connectedSsid))
            {
                if (connectedSsid.Equals(AvailableNetwork.Ssid))
                {
                    connectivityLevel = connectedProfile.GetNetworkConnectivityLevel().ToString();
                }
            }

            ConnectivityLevel = connectivityLevel;
        }
    }
}
