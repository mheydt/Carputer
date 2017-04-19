using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Carputer.Phone.UWP.Interfaces;
using Carputer.Phone.UWP.Models;
using Carputer.Phone.UWP.Rx;

namespace Carputer.Phone.UWP.Services
{
    public interface ILocationService
    {
    }

    public class LocationUpdate
    {
        public Geoposition Position { get; private set; }

        public LocationUpdate(Geoposition geoposition)
        {
            Position = geoposition;
        }
    }

    public class LocationService : ILocationService, INeedInitialization, INeedShutdown, IObservable<LocationUpdate>
    {
        private List<IObserver<LocationUpdate>> _observers = new List<IObserver<LocationUpdate>>();
        private Geolocator _geolocator;

        public async Task InitializeAsync()
        {
            var access = await Geolocator.RequestAccessAsync();
            if (access == GeolocationAccessStatus.Allowed)
            {
                _geolocator = new Geolocator() {DesiredAccuracyInMeters = 1};
                _geolocator.StatusChanged += _geolocator_StatusChanged;
                _geolocator.PositionChanged += _geolocator_PositionChanged;
                var location = await _geolocator.GetGeopositionAsync();
                updateLocation(location);
            }
        }

        private void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            updateLocation(args.Position);
        }

        private void _geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
        }

        private void updateLocation(Geoposition geoposition)
        {
            foreach (var o in _observers)
            {
                try
                {
                    o.OnNext(new LocationUpdate(geoposition));
                }
                catch (Exception e)
                {
                }
            }
        }

        public async Task ShutdownAsync()
        {
            await Task.CompletedTask;
        }

        public IDisposable Subscribe(IObserver<LocationUpdate> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
            return new Unsubscriber<LocationUpdate>(_observers, observer);
        }
    }
}
