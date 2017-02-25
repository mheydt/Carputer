using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Carputer.UWP.Util.Serilog.Udp;
using Serilog;
using Serilog.Core;

namespace Carputer.UWP.Devices.GPS.NMEA
{
    public class NmeaGpsDevice
    {
        public IObservable<GlobalPositioningSystemFixData> Positions { get; private set; }
        private Subject<GlobalPositioningSystemFixData> _positionsSubject;

        private PiUartGpsDevice _gps;
        private IDisposable _subscription;
        private Logger _logger;

        public bool InitializedSuccessfully
        {
            get { return _gps.InitializedSuccessfully; }
        }

        public NmeaGpsDevice()
        {
            _positionsSubject = new Subject<GlobalPositioningSystemFixData>();
            Positions = _positionsSubject;

            _gps = new PiUartGpsDevice();

            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Udp(IPAddress.Parse("192.168.0.26"), 9998)
                .CreateLogger();

        }

        public async Task InitializeAsync()
        {
            await _gps.InitializeAsync();
            if (_gps.InitializedSuccessfully)
            {
                _subscription = _gps.Readings.Subscribe(handleMessage);
            }
        }

        public async Task ShutdownAsync()
        {
            if (_gps.InitializedSuccessfully)
            {
                if (_subscription != null)
                {
                    _subscription.Dispose();
                    _subscription = null;
                }
            }
            await _gps.ShutdownAsync();
        }

        private void handleMessage(string message)
        {
            _logger.Information($"Got gps data....: {GlobalPositioningSystemFixData.Identifier} {message}");
            if (message.StartsWith(GlobalPositioningSystemFixData.Identifier + ","))
            {
                _logger.Information("Parsing gps data");

                var parts = message.Split(new[] { ',', '*' });

                if (parts.Skip(1).Take(4).All(i => !string.IsNullOrEmpty(i)))
                {
                    try
                    {
                        var msg = new GlobalPositioningSystemFixData(message, _logger);

                        _logger.Information("Publishing gps data");

                        _positionsSubject.OnNext(msg);

                    }
                    catch (Exception e)
                    {
                        _logger.Information(e.Message);
                    }
                }
                else
                {
                    _logger.Information("Incomplete gps data in message, so ignoring");
                }
            }
        }
    }
}
