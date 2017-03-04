using App1;
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

        private Client _client;
        private Server _server;

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

            /*
            _cts = new CancellationTokenSource();

            _state = new Dictionary<string, string>();

            if (_simulatorMode)
            {
                _processTask = Task.Run(() => simulate(_cts.Token));
            }
            else
            {
                _processTask = Task.Run(() => startPollingDevice(_cts.Token));
            }
            */

            _client = new Client();
            //_server = new Server();

            await _client.InitAsync();
            //await _server.InitAsync();

            _cts = new CancellationTokenSource();
            _processTask = Task.Run(() => startPollingDevice(_cts.Token));

            Tracer.writeLine("init out");
        }

        /*
        Task.Run(() => executor());
        }

        private void executor()
        {
            connectAsync().Wait();
            write("ATZ");
            var r = read();
            write("ATE0");
            r = read();
            write("ATL1");
            r = read();
            write("ATSP00");
            r = read();

            write("0902");
            r = read();
            //var t = getVIN();
            //t.Wait();
            //var vin = t.Result;
            //_state["vin"] = vin;

            Tracer.writeLine("done");
        }
        */
        /*
        private void write(string data)
        {
            _transport.WriteAsync(data + "\r").Wait();
        }

        private string read()
        {
            var t = _transport.ReadAsync();
            t.Wait();
            var data = t.Result;
            return data;
        }
        */
        /*

            if (_cts != null) throw new Exception("Already running");

            _simulatorMode = simulatormode;

            _cts = new CancellationTokenSource();

            _state = new Dictionary<string, string>();

            if (_simulatorMode)
            {
                _processTask = Task.Run(() => simulate(_cts.Token));
            }
            else
            {
                _processTask = Task.Run(() => startPollingDevice(_cts.Token));
            }
            Tracer.writeLine("init out");
        }
        */

        public async Task ShutdownAsync()
        {
            //if (_cts == null) return;

            Tracer.writeLine("Taking down OBD-II");

            _cts.Cancel();
            _processTask.Wait();

            _cts = null;
            _processTask = null;

            //await _transport.DisconnectAsync();
            await _client.ShutdownAsync();

            Tracer.writeLine("OBD-II has been taken down");
        }

        private async void simulate(CancellationToken token)
        {
            await _server.InitAsync();

            while (!token.IsCancellationRequested)
            {
                foreach (var cmd in _pids.Keys)
                {
                    if (token.IsCancellationRequested) break;

                    var key = _pids[cmd];

                    var s = ObdUtils.GetEmulatorValue(cmd);
                    update(key, s);
                }
            }
            await _server.ShutdownAsync();

            Tracer.writeLine("simulate out");
        }

        private async Task startPollingDevice(CancellationToken token)
        {
            //await connectAsync(token);
            await initializeDeviceAsync(token);
            await pollDeviceAsync(token);
        }

        private async Task connectAsync(CancellationToken token = default(CancellationToken))
        {
            Tracer.writeLine("Attempting to connect");

            //await _transport.ConnectAsync(token);

            Tracer.writeLine("Connected");
        }

        private async Task initializeDeviceAsync(CancellationToken token)
        { 
            Tracer.writeLine("Initializing OBD-II");
            // initialize the device

            await _client.ExecuteCommand("ATZ");
            await _client.ExecuteCommand("ATE0");
            await _client.ExecuteCommand("ATL1");
            await _client.ExecuteCommand("ATSP00");
            //await executeAsync("ATZ", token);
            //await executeAsync("ATE0", token);
            //await executeAsync("ATL1", token);
            //await executeAsync("ATSP00", token);

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
                            //var s = await executeAsync(cmd, token);
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

        private async Task<string> executeAsync(string command, CancellationToken token = default(CancellationToken))
        {
            try
            {
                Tracer.writeLine("execute async 1 " + command);

                await _transport.WriteAsync(command + "\r", token);
                Tracer.writeLine("execute async 3: " + command);

                Tracer.writeLine("Waiting for response");

                var ret = await _transport.ReadAsync(token);

                Tracer.writeLine("the response is: " + ret);

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

                Tracer.writeLine("executeAsync reply is: " + ret);

                return ret.Trim();
            }
            catch (Exception ex)
            {
                Tracer.writeLine("Exception in executeAsync: " + ex.Message);
                throw;
            }
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
                //await executeAsync("0902", token);

            if (result == "UNABLE TO CONNECT") throw new Exception("Unable to connect to ECN");

            if (!result.StartsWith("49")) throw new Exception("Could not connect / get VIN");

            /*
            Tracer.writeLine($"Started with 49 - reading more");
            while (!result.Contains("49 02 05"))
            {
                string tmp = await _transport.ReadAsync(token);
                result += tmp;
            }
            */

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
