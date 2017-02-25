using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;
using Caliburn.Micro;
using Carputer.UWP.Devices.GPS;
using Carputer.UWP.Services;
using PropertyChanged;

namespace Carputer.UWP.ViewModels
{
    [ImplementPropertyChanged]
    public class MapViewModel : Screen
    {
        public class MapViewModelMemento
        {
            public double ZoomLevel { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Direction { get; set; }

            public MapViewModelMemento()
            {
            }

            public MapViewModelMemento(MapViewModel vm)
            {
                ZoomLevel = vm.ZoomLevel;
                Latitude = vm.Center.Latitude;
                Longitude = vm.Center.Longitude;
                Direction = vm.Direction;
            }

            public MapViewModelMemento(double latitude, double longitude, double direction, double zoomlevel)
            {
                ZoomLevel = zoomlevel;
                Latitude = latitude;
                Longitude = longitude;
            }
        }

        private IGPSService _gpsService;

        public Carputer.UWP.Devices.GPS.GpsLocation Center { get; set; }

        private double _zoomLevel;
        private ISettingsService _settingsService;
        private IDisposable _subsciption;

        public double ZoomLevel
        {
            get {  return _zoomLevel; }
            set
            {
                _zoomLevel = value;
            }
        }

        public double Direction { get; set; }

        public MapStyle Style { get; set; } = MapStyle.AerialWithRoads;
        public MapViewModel(IGPSService gpsService, ISettingsService settingsService)
        {
            _gpsService = gpsService;
            _settingsService = settingsService;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _settingsService.GetAsync(nameof(MapViewModel),
                new MapViewModelMemento(_gpsService.State.Latitude, _gpsService.State.Longitude, 0, 15.07)
            ).ContinueWith(a =>
            {
                var m = a.Result;
                Center =
                    new Carputer.UWP.Devices.GPS.GpsLocation
                    {
                        Longitude = m.Longitude,
                        Latitude = m.Latitude
                    };
                ZoomLevel = m.ZoomLevel;
            });

            _subsciption = _gpsService.LocationEvents.ObserveOnDispatcher(CoreDispatcherPriority.Normal).Subscribe(gotNewGpsLocation);
        }

        private void gotNewGpsLocation(GpsLocationEvent locationEvt)
        {
            Center = new GpsLocation(locationEvt.Location.Latitude, locationEvt.Location.Longitude);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (_subsciption != null)
            {
                _subsciption.Dispose();
                _subsciption = null;
            }

            _settingsService.PutAsync(nameof(MapViewModel), new MapViewModelMemento(this));
        }
    }
}
