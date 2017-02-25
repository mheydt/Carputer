using ST.Fx.Gps.NMEA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

//[assembly: Xamarin.Forms.Dependency (typeof(ST.Fx.Gps.NMEA.UWP.UwpNmeaGpsService))]

namespace ST.Fx.Gps.NMEA.UWP
{
    public class UwpNmeaGpsService : IGpsService
    {
        public IObservable<GlobalPositioningSystemFixData> Positions { get; private set; }
        private Subject<GlobalPositioningSystemFixData> _positionsSubject;

        private PiUartGpsDevice _gps;

        public UwpNmeaGpsService()
        {
            _positionsSubject = new Subject<GlobalPositioningSystemFixData>();
            Positions = _positionsSubject;

            _gps = new PiUartGpsDevice();
            _gps.Readings.Subscribe(handleMessage);
        }

        public async Task InitializeAsync()
        {
            _gps.initializeAsync();
        }

        public async Task ShutdownAsync()
        {
        }

        private void handleMessage(string message)
        {
            if (message.StartsWith(GlobalPositioningSystemFixData.Identifier + ","))
            {
                var msg = new GlobalPositioningSystemFixData(message);
                _positionsSubject.OnNext(msg);
            }
        }
    }
}
