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
        private Dictionary<string, string> _state;
        private CancellationTokenSource _cts;
        const string _defaultValue = "-255";
        private Dictionary<string, string> _pids;
        private bool _simulatorMode = false;
        private IOBD2Transport _transport;
        private int _pollInterval = 5000;
        private Task _pollTask;
        private object _lock = new object();

        public OBDIIService(IOBD2Transport transport)
        {
            _transport = transport;
        }

        public async Task<bool> InitAsync(bool simulatormode = false)
        {
            Tracer.writeLine("init");

            if (_cts != null) throw new Exception("Already running");

            _state = new Dictionary<string, string>();
            
            _pids = ObdUtils.GetPIDs();
            foreach (var v in _pids.Values)
            {
                _state.Add(v, _defaultValue);
            }

            _simulatorMode = simulatormode;

            if (simulatormode)
            {
                startPolling();

                return true;
            }

            if (!await _transport.ConnectAsync())
            { 
                return false;
            }

            // initialize the device
            await executeAsync("ATZ\r");
            await executeAsync("ATE0\r");
            await executeAsync("ATL1\r");
            await executeAsync("ATSP00\r");

            startPolling();

            return true;
        }

        public async Task DisconnectAsync()
        {
            if (_cts == null) return;
            {
                _cts.Cancel();
                _pollTask.Wait();
                _cts = null;
            }
            await _transport.DisconnectAsync();
        }

        private void startPolling()
        {
            _cts = new CancellationTokenSource();

            _pollTask = Task.Run(async () => await pollerAsync());
        }

        private async Task pollerAsync()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    //if (_cts.IsCancellationRequested) _cts.Token.ThrowIfCancellationRequested();

                    if (!_state.ContainsKey("vin"))
                    {
                        var vin = _simulatorMode ? "SIMULATOR" : await getVIN();
                        Tracer.writeLine($"Vin=={vin}");

                        update("vin", vin);
                    }

                    //Tracer.writeLine("poll");

                    var s = "";
                    foreach (var cmd in _pids.Keys)
                    {
                        if (_cts.IsCancellationRequested) break;

                        var key = _pids[cmd];

                        s = (_simulatorMode) ? s = ObdUtils.GetEmulatorValue(cmd) : await executeAsync(cmd);
                        if (s != "ERROR")
                        {
                            update(key, s);
                        }
                    }

                    if (!_cts.IsCancellationRequested)
                    {
                        try
                        {
                            Task.Delay(_pollInterval, _cts.Token).Wait(_cts.Token);
                            //Task.Delay(_pollInterval).Wait();
                        }
                        catch (Exception)
                        {
                            //_cts.Token.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
            finally
            {

            }
        }

        private async Task<string> executeAsync(string command)
        {
            if (!await _transport.WriteAsync(command)) throw new Exception("Error executing command: " + command);

            Task.Delay(100).Wait();

            var reply = await readResponseAsync(command);
            
            return "";
        }

        private async Task<string> readResponseAsync(string command = "", bool waitFormTerminator = true)
        {
            var ret = await _transport.ReadAsync();
            while (!ret.Trim().EndsWith(">"))
            {
                var next = await _transport.ReadAsync();
                ret = ret + next;
            }

            ret = ret.Trim();
            ret = ret.Replace("SEARCHING...", "").Replace("\r\n", "");

            if (ret.StartsWith(command))
            {
                ret = ret.Substring(command.Length);
            }

            return ret;
        }

        private async Task<string> getVIN()
        {
            Tracer.writeLine("In getVIN");
            var result = await executeAsync("0902\r");
            if (result.StartsWith("49"))
            {
                Tracer.writeLine($"Started with 49 - reading more");
                while (!result.Contains("49 02 05"))
                {
                    string tmp = await _transport.ReadAsync();
                    result += tmp;
                }
            }
            Tracer.writeLine("getVIN calling parse: " + result);
            return ObdUtils.ParseVINMsg(result);
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
