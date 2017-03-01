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

        public OBDIIService(IOBD2Transport transport)
        {
            _transport = transport;

            _pids = ObdUtils.GetPIDs();
            foreach (var v in _pids.Values)
            {
                _state.Add(v, _defaultValue);
            }
        }

        public async Task InitAsync(bool simulatormode = false)
        {
            Tracer.writeLine("init");

            if (_cts != null) throw new Exception("Already running");

            _simulatorMode = simulatormode;

            _cts = new CancellationTokenSource();
            _processTask = Task.Run(async () => await processAsync(_cts.Token));
        }

        public async Task ShutdownAsync()
        {
            if (_cts == null) return;

            Tracer.writeLine("Taking down OBD-II");

            _cts.Cancel();
            _processTask.Wait();

            _cts = null;
            _processTask = null;

            await _transport.DisconnectAsync();

            Tracer.writeLine("OBD-II has been taken down");
        }

        private async Task processAsync(CancellationToken token = default(CancellationToken))
        {
            _state = new Dictionary<string, string>();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_simulatorMode)
                    {
                        await pollDevice(token);
                    }
                    else
                    {
                        var connected = await connectAsync(token);
                        if (connected)
                        {
                            await pollDevice(token);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tracer.writeLine(ex.Message);
            }
            Tracer.writeLine("processing stopped");
        }

        private async Task<bool> connectAsync(CancellationToken token = default(CancellationToken))
        {
            Tracer.writeLine("Attempting to connect");
            if (!await _transport.ConnectAsync(token))
            {
                Tracer.writeLine("Could not connect");
                return false;
            }

            try
            {
                Tracer.writeLine("Initializing OBD-II");
                // initialize the device
                var r = await executeAsync("ATZ", token);
                r = await executeAsync("ATE0", token);
                r = await executeAsync("ATL1", token);
                r = await executeAsync("ATSP00", token);

                var vin = await getVIN(token);

                Tracer.writeLine($"Vin=={vin}");
            }
            catch (Exception ex)
            {
                Tracer.writeLine("Exception initializing: " + ex.Message);
                await _transport.DisconnectAsync(token);
                return false;
            }
            Tracer.writeLine("Connected");

            return true;
        }

        private async Task pollDevice(CancellationToken token)
        {
            Tracer.writeLine("Starting to poll device");

            while (!token.IsCancellationRequested)
            {
                foreach (var cmd in _pids.Keys)
                {
                    if (token.IsCancellationRequested) break;

                    var key = _pids[cmd];

                    try
                    {
                        var s = "";
                        if (_simulatorMode)
                        {
                            s = ObdUtils.GetEmulatorValue(cmd);
                        }
                        else
                        {
                            s = await executeAsync(cmd, token);
                        }

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

            Tracer.writeLine("polling of device ended");
        }

        private async Task<string> executeAsync(string command, CancellationToken token = default(CancellationToken))
        {
            try
            {
                if (!await _transport.WriteAsync(command + "\r", token)) throw new Exception("Error executing command: " + command);

                await Task.Delay(100, token);

                var reply = await readResponseAsync(command, token);

                return reply;
            }
            catch (Exception ex)
            {
                Tracer.writeLine("Exception in executeAsync: " + ex.Message);
                throw;
            }

        }

        private async Task<string> readResponseAsync(string command = "", CancellationToken token = default(CancellationToken))
        {
            var ret = await _transport.ReadAsync(token);
            while (!ret.Trim().EndsWith(">"))
            {
                var next = await _transport.ReadAsync(token);
                ret = ret + next;
            }

            ret = ret.Trim();
            ret = ret.Replace("SEARCHING...", "").Replace("\r\n", " ").Replace("\r", " ");

            if (ret.EndsWith(">")) ret = ret.Substring(0, ret.Length - 1);

            if (ret.StartsWith(command))
            {
                ret = ret.Substring(command.Length);
            }

            return ret.Trim();
        }

        private async Task<string> getVIN(CancellationToken token = default(CancellationToken))
        {
            Tracer.writeLine("In getVIN");

            if (_simulatorMode)
            {
                _state["vin"] = "SIMULATOR";
                return _state["vin"];
            }

            var result = await executeAsync("0902", token);

            if (result == "UNABLE TO CONNECT") throw new Exception("Unable to connect to ECN");

            if (!result.StartsWith("49")) throw new Exception("Could not connect / get VIN");

            Tracer.writeLine($"Started with 49 - reading more");
            while (!result.Contains("49 02 05"))
            {
                string tmp = await _transport.ReadAsync(token);
                result += tmp;
            }

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
