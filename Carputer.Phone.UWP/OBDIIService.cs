using App1;
using Carputer.Phone.UWP.OBDII;
using Sockets.Plugin;
using ST.Fx.Debug.Tracer;
using ST.Fx.OBDII.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public class OBDIIService : IOBDIIService
    {
        private Dictionary<string, string> _state = new Dictionary<string, string>();
        private CancellationTokenSource _cts;
        const string _defaultValue = "-255";
        private Dictionary<string, string> _pids;
        private bool _simulatorMode = false;
        private IOBD2Transport _transport;
        private int _pollInterval = 5000;
        private object _lock = new object();
        private Task _connectTask = null;
        private int _connectionAttemptInterval = 5000;
        private Task _processTask;

        private IOBDIITransport _client;
        private IOBDIIServer _server;
        private Func<IOBDIITransport> _transportFactory;
        private Func<IOBDIIServer> _serverFactory;

        public OBDIIService(
            Func<IOBDIITransport> transportFactory,
            Func<IOBDIIServer> serverFactory = null)
        {
            _transportFactory = transportFactory;
            _serverFactory = serverFactory;

            _pids = ObdUtils.GetPIDs();
            foreach (var v in _pids.Values)
            {
                _state.Add(v, _defaultValue);
            }
        }

        public async Task InitAsync()
        {
            Tracer.writeLine("init");

            if (_cts != null) throw new Exception("Already running");

            _client = _transportFactory();
            _server = _serverFactory == null ? new NullOBDIIServer() : _serverFactory();

            _cts = new CancellationTokenSource();

            await _client.InitAsync(_cts.Token);
            await _server.InitAsync(_cts.Token);

            _processTask = Task.Run(() => startPollingDevice(_cts.Token));

            Tracer.writeLine("init out");
        }

        public async Task ShutdownAsync()
        {
            //if (_cts == null) return;

            Tracer.writeLine("Taking down OBD-II");

            _cts.Cancel();
            _processTask.Wait();

            _cts = null;
            _processTask = null;

            await _server.ShutdownAsync();
            await _client.ShutdownAsync();

            Tracer.writeLine("OBD-II has been taken down");
        }

        private async Task startPollingDevice(CancellationToken token)
        {
            await initializeDeviceAsync(token);
            await pollDeviceAsync(token);
        }

        private async Task initializeDeviceAsync(CancellationToken token)
        { 
            Tracer.writeLine("Initializing OBD-II");

            try
            {
                await _client.ExecuteCommand("ATZ");
                await _client.ExecuteCommand("ATE0");
                await _client.ExecuteCommand("ATL1");
                await _client.ExecuteCommand("ATSP00");
            }
            catch (Exception ex)
            {

            }

            var vin = await getVIN();
            _state["vin"] = vin;

            Tracer.writeLine($"Vin=={vin}");
        }

        private async Task pollDeviceAsync(CancellationToken token)
        {
            Tracer.writeLine("Starting to poll device");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var cmd in _pids.Keys)
                    {
                        if (token.IsCancellationRequested) break;

                        var key = _pids[cmd];

                        try
                        {
                            var s = await _client.ExecuteCommand(cmd);
                            if (s != "ERROR")
                            {
                                Tracer.writeLine($"{key} {s}");
                                update(key, s);
                            }
                        }
                        catch (Exception ex)
                        {
                            // transport error - kick out and try to connect again
                            Tracer.writeLine(ex.Message);
                            return;
                        }
                    }

                    if (!token.IsCancellationRequested)
                    {
                        Task.Delay(_pollInterval, token).Wait(token);
                    }
                }
            }
            catch (Exception ex)
            {
                Tracer.writeLine("Exception in poller: " + ex.Message);
            }

            Tracer.writeLine("polling of device ended");
        }

        private async Task<string> getVIN(CancellationToken token = default(CancellationToken))
        {
            Tracer.writeLine("In getVIN");

            if (_simulatorMode)
            {
                _state["vin"] = "SIMULATOR";
                return _state["vin"];
            }

            var result = await _client.ExecuteCommand("0902");

            if (result == "UNABLE TO CONNECT") throw new Exception("Unable to connect to ECN");

            if (!result.StartsWith("49")) throw new Exception("Could not connect / get VIN");

            Tracer.writeLine("getVIN calling parse: " + result);
            var vin = ObdUtils.ParseVINMsg(result);

            if (vin == "ERROR") throw new Exception("Could not get VIN");

            update("vin", vin);

            return vin;
        }

        private void update(string key, string value)
        {
            lock (_lock)
            {
                _state[key] = value;
            }
        }

        public Dictionary<string, string> GetState()
        {
            if (!_simulatorMode && _cts == null)
            {
                //if there is no connection
                return null;
            }

            var ret = new Dictionary<string, string>();
            lock (_lock)
            {
                foreach (var key in _state.Keys)
                {
                    ret.Add(key, _state[key]);
                }
            }

            return ret;
        }
    }
}
