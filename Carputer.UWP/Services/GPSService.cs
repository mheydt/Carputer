using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Carputer.UWP.Devices.GPS;
using Carputer.UWP.Devices.GPS.NMEA;
using Carputer.UWP.Interfaces;

namespace Carputer.UWP.Services
{
    public interface IGPSService
    {
        ISubject<GpsLocationEvent> LocationEvents { get; set; }
        GPSService.GPSServiceState State { get; }
    }

    public class GPSService : IGPSService, IService
    {
        public class GPSServiceState
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Altitiude { get; set; }
        }

        private GPSServiceState _state;
        public GPSServiceState State {  get { return _state; } }

        public ISubject<GpsLocationEvent> LocationEvents { get; set; }
        private Subject<GpsLocationEvent> _locationEvents;

        private ISettingsService _settingsService;

        private NmeaGpsDevice _nmeaGpsDevice;

        public GPSService(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            _locationEvents = new Subject<GpsLocationEvent>();
            LocationEvents = _locationEvents;

            _nmeaGpsDevice = new NmeaGpsDevice();
        }

        public async Task StartAsync()
        {
            _state = await _settingsService.GetAsync(
                "Service.GPS.State",
                new GPSServiceState()
                {
                    Latitude = 46.935125833,
                    Longitude = -114.07040899,
                });

            _state = new GPSServiceState()
            {
                Latitude = 46.935125833,
                Longitude = -114.07040899,
            };

            var df = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            switch (df)
            {
                case "Windows.IOT":
                    break;

                case "Windows.Desktop":
                    //simulateMovement();



                    break;
            }

            await _settingsService.PutAsync("Service.GPS.State", _state);

            await _nmeaGpsDevice.InitializeAsync();
        }

        public async Task StopAsync()
        {
            await _settingsService.PutAsync("Service.GPS.State", _state);
        }

        private void simulateMovement()
        {
            Task.Delay(5000).ContinueWith(
                a =>
                {
                    _state.Latitude -= 0.0001;
                    _locationEvents.OnNext(new GpsLocationEvent(_state.Latitude, _state.Longitude));
                    simulateMovement();
                });
        }
    }
}
