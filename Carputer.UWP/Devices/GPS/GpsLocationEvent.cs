using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Background;

namespace Carputer.UWP.Devices.GPS
{
    public class GpsLocationEvent
    {
        public DateTime Timestamp { get; set; }
        public GpsLocation Location { get; set; }

        public GpsLocationEvent()
        {
            
        }

        public GpsLocationEvent(DateTime timestamp, double latitude, double longitude)
        {
            Timestamp = timestamp;
            Location = new GpsLocation(latitude, longitude);
        }
        public GpsLocationEvent(double latitude, double longitude) : this(DateTime.Now, latitude, longitude)
        {
        }
    }
}
