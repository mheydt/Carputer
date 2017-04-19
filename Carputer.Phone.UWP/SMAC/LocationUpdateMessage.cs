using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.Phone.UWP.SMAC
{
    public class LocationUpdateMessage
    {
        public DateTime DateTimeUTC { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
    }
}
