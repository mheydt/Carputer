using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Carputer.Phone.BkgTasks.UWP;
using Carputer.Phone.UWP.Interfaces;
using Windows.Networking.Sockets;
using Caliburn.Micro;
using Carputer.Phone.UWP.SMAC;

namespace Carputer.Phone.UWP.Services
{
    public interface ICarputerProxyService : IService
    {
        
    }

    public class CarputerProxyService : ICarputerProxyService
    {
        private DatagramSocket _socket;
        private ILocationService _locationService;
        private IDisposable _locationSubscription;
        private MessageFacade _messageFacade;

        public CarputerProxyService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task InitializeAsync()
        {
            BackgroundServiceTaskHelper.register<CarputerBkgServicesTask>("CarputerProxyService");

            _messageFacade = new MessageFacade();
            await _messageFacade.InitializeAsync();

            await (_locationService as INeedInitialization)?.InitializeAsync();

            var locationObservable = _locationService as IObservable<LocationUpdate>;
            if (locationObservable != null)
            {
                _locationSubscription = locationObservable.Subscribe(
                    l =>
                    {
                        
                    });
            }

            await startUdpListenerAsync();
        }

        public async Task ShutdownAsync()
        {
            await (_locationService as INeedShutdown)?.ShutdownAsync();
            _locationSubscription?.Dispose();
            await Task.CompletedTask;
        }


        private async Task startUdpListenerAsync()
        {
            try
            {
                var ip = GetLocalIp();
                //var bcst = GetBroadcastAddress(ip, ip);

                _socket = new DatagramSocket();
                _socket.MessageReceived += _socket_MessageReceived;
                _socket.Control.MulticastOnly = true;
                await _socket.BindServiceNameAsync("15000");
                var remoteAddress = "224.0.0.0";
                _socket.JoinMulticastGroup(new Windows.Networking.HostName(remoteAddress));
            }
            catch (Exception ex)
            {

            }
        }

        private string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();
            var hostnames = NetworkInformation.GetHostNames().ToArray();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }
        /*
        public static IPAddress GetBroadcastAddress(UnicastIPAddressInformation unicastAddress)
        {
            return GetBroadcastAddress(unicastAddress.Address, unicastAddress.IPv4Mask);
        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            uint ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            uint broadCastIpAddress = ipAddress | ~ipMaskV4;

            return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
        }
        */

        private void _socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var len = args.GetDataReader().UnconsumedBufferLength;
            var msg = args.GetDataReader().ReadString(len);
            Debug.WriteLine(msg);
        }
    }
}
