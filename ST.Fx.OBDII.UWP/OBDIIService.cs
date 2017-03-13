using ST.Fx.Debug.Tracer;
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
    public class OBDIIService : IOBDIIService, IObservable<OBDIIUpdate>
    {
        public class Unsubscriber : IDisposable
        {
            private IObserver<OBDIIUpdate> _observer;
            private List<IObserver<OBDIIUpdate>> _observers;

            public Unsubscriber(List<IObserver<OBDIIUpdate>> observers, IObserver<OBDIIUpdate> observer)
            {
                _observers = observers;
                _observer = observer;
            }
            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        private List<IObserver<OBDIIUpdate>> _observers = new List<IObserver<OBDIIUpdate>>();

        private Dictionary<string, string> _state = new Dictionary<string, string>();
        private CancellationTokenSource _cts;
        const string _defaultValue = "-255";
        private Dictionary<string, string> _pids;
        private bool _simulatorMode = false;
        private int _pollInterval = 100;
        private object _lock = new object();
        private Task _connectTask = null;
        private int _connectionAttemptInterval = 5000;
        private Task _processTask;

        private IOBDIITransport _transport;
        private IOBDIIServer _server;

        public OBDIIService(
            IOBDIITransport transport,
            IOBDIIServer server)
        {
            _transport = transport;
            _server = server;

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

            _cts = new CancellationTokenSource();

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
            await _transport.ShutdownAsync();

            Tracer.writeLine("OBD-II has been taken down");
        }

        private async Task startPollingDevice(CancellationToken token)
        {
            while (!_cts.IsCancellationRequested)
            {
                await _transport.InitAsync(_cts.Token);
                await _server.InitAsync(_cts.Token);

                try
                {
                    await initializeDeviceAsync(token);
                }
                catch (Exception ex)
                {
                    await _transport.ShutdownAsync();
                    await _server.ShutdownAsync();
                    continue;
                }

                await pollDeviceAsync(token);
            }
        }

        private async Task<bool> initializeDeviceAsync(CancellationToken token)
        { 
            Tracer.writeLine("Initializing OBD-II");

            try
            {
                await _transport.ExecuteCommand("ATZ");
                await Task.Delay(100000);
                await _transport.ExecuteCommand("ATE0");
                await _transport.ExecuteCommand("ATL1");
                await _transport.ExecuteCommand("ATSP00");
            }
            catch (Exception ex)
            {
                return false;
            }

            // need to handle "UNABLE TO CONNECT"

            var vin = await getVIN();
            _state["vin"] = vin;

            Tracer.writeLine($"Vin=={vin}");

            return true;
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
                            var s = await _transport.ExecuteCommand(cmd);
                            if (s != "ERROR")
                            {
                                s = ObdUtils.ParseObd01Msg(s);
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

                    notify();

                    if (!token.IsCancellationRequested)
                    {
                        //Task.Delay(_pollInterval, token).Wait(token);
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

            var result = await _transport.ExecuteCommand("0902");

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
            _state[key] = value;
        }

        private void notify()
        {
            var update = new OBDIIUpdate(_state);

            foreach (var observer in _observers)
            {
                observer.OnNext(update);
            }
        }

        /*
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
        */

        public IDisposable Subscribe(IObserver<OBDIIUpdate> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }
    }
}
