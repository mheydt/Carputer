using Sockets.Plugin;
using ST.Fx.OBDII.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Fx.OBDII.Wifi.UWP
{
    public class OBDIIService : IOBDIIService
    {
        const string DefValue = "-255";
        private byte[] _buffer = new byte[1024];

        private readonly Object _lock = new Object();
        private Dictionary<string, string> _data;
        private Dictionary<string, string> _piDs;
        private bool _simulatormode;
        private CancellationTokenSource _cts;
        private TcpSocketClient _socketClient;

        public async Task<bool> InitAsync(bool simulatormode = false)
        {
            if (_cts != null) throw new Exception("Already running");

            _data = new Dictionary<string, string> { { "vin", DefValue } };
            
            _piDs = ObdUtils.GetPIDs();
            foreach (var v in _piDs.Values)
            {
                _data.Add(v, DefValue);
            }

            _simulatormode = simulatormode;

            if (simulatormode)
            {
                _cts = new CancellationTokenSource();

                PollObdAsync();

                return true;
            }

            var isObdReaderAvailable = false;
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        var ipaddr = addrInfo.Address;
                        if (ipaddr.ToString().StartsWith("192.168.0"))
                        {
                            isObdReaderAvailable = true;
                            break;
                        }
                    }
                }
            }

            if (!isObdReaderAvailable)
            {
                return false;
            }

            if (!await connectAsync())
            {
                _socketClient = null;
                return false;
            }

            await SendAndReceiveAsync("ATZ\r");
            await SendAndReceiveAsync("ATE0\r");
            await SendAndReceiveAsync("ATL1\r");
            await SendAndReceiveAsync("ATSP00\r");

            PollObdAsync();

            return true;
        }

        public Dictionary<string, string> Read()
        {
            if (!_simulatormode && _cts == null)
            {
                //if there is no connection
                return null;
            }

            var ret = new Dictionary<string, string>();
            lock (_lock)
            {
                foreach (var key in _data.Keys)
                {
                    ret.Add(key, _data[key]);
                }
                foreach (var v in _piDs.Values)
                {
                    _data[v] = DefValue;
                }
            }
            return ret;
        }

        private async Task PollObdAsync()
        {
            if (_cts == null) throw new Exception("Not initialized");
            if (_socketClient == null) throw new Exception("Not connected");

            if (_cts.Token.IsCancellationRequested) return;

            try
            {
                var s = _simulatormode ? "SIMULATORIPHONE12" : await GetVIN();
                lock (_lock)
                {
                    _data["vin"] = s;
                }
                foreach (var cmd in _piDs.Keys)
                {
                    var key = _piDs[cmd];

                    s = (_simulatormode) ? s = ObdUtils.GetEmulatorValue(cmd) : await RunCmdAsync(cmd);
                    if (s != "ERROR")
                    {
                        lock (_lock)
                        {
                            _data[key] = s;
                        }
                    }
                    if (_cts.Token.IsCancellationRequested) return;
                }

                if (!_cts.IsCancellationRequested)
                {
                    Task.Delay(100, _cts.Token).ContinueWith(_ => PollObdAsync(), _cts.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _cts = null;

                _socketClient.Dispose();
                _socketClient = null;
            }
        }

        private async Task<string> GetVIN()
        {
            var result = await SendAndReceiveAsync("0902\r");
            if (result.StartsWith("49"))
            {
                while (!result.Contains("49 02 05"))
                {
                    string tmp = await ReceiveAsync();
                    result += tmp;
                }
            }
            return ObdUtils.ParseVINMsg(result);
        }

        private async Task<bool> connectAsync()
        {
            try
            {
                _socketClient = new TcpSocketClient();
                await _socketClient.ConnectAsync("192.168.0.10", 35000);
            }
            catch (Exception ex)
            {
                _socketClient = null;
            }

            return _socketClient != null;
        }

        private async Task<string> SendAndReceiveAsync(string msg)
        {
            try
            {
                await WriteAsync(msg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            await Task.Delay(100);
            try
            {
                var s = await ReceiveAsync();
                Debug.WriteLine("Received: " + s);
                s = s.Replace("SEARCHING...\r\n", "");
                return s;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return "";
        }

        private async Task WriteAsync(string msg)
        {
            Debug.WriteLine(msg);
            var buffer = Encoding.ASCII.GetBytes(msg);
            await _socketClient.WriteStream.WriteAsync(buffer, 0, buffer.Length);
            _socketClient.WriteStream.Flush();
        }

        private async Task<string> ReceiveAsync()
        {
            var ret = await ReceiveAsyncRawAsync();
            while (!ret.Trim().EndsWith(">"))
            {
                var tmp = await ReceiveAsyncRawAsync();
                ret = ret + tmp;
            }
            return ret;
        }

        private async Task<string> ReceiveAsyncRawAsync()
        {
            var bytes = await _socketClient.ReadStream.ReadAsync(_buffer, 0, _buffer.Length);
            var s = Encoding.ASCII.GetString(_buffer, 0, bytes);
            Debug.WriteLine(s);
            return s;
        }

        private async Task<string> RunCmdAsync(string cmd)
        {
            var result = await SendAndReceiveAsync(cmd + "\r");
            return ObdUtils.ParseObd01Msg(result);
        }

        public async Task DisconnectAsync()
        {
            if (_socketClient != null)
            {
                _socketClient.Dispose();
                _socketClient = null;
            }
        }
    }
}
