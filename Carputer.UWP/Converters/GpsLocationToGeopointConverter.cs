using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;
using Carputer.UWP.Devices.GPS;

namespace Carputer.UWP.Converters
{
    public class GpsLocationToGeopointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new Geopoint(new BasicGeoposition());
            var gpsLocation = value as GpsLocation;
            return new Geopoint(new BasicGeoposition() {Latitude = gpsLocation.Latitude, Longitude = gpsLocation.Longitude});
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var geo = value as Geopoint;
            if (geo == null) return new GpsLocation();
            return new GpsLocation(geo.Position.Latitude, geo.Position.Longitude);
        }
    }
}
