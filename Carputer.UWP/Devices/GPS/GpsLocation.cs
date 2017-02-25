using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.UWP.Devices.GPS
{
    public class GpsLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public GpsLocation()
        {
            
        }

        public GpsLocation(double latitude, double longitude, double altitude=0.0)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
    }
}
