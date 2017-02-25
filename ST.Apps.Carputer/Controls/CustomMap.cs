using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace ST.Apps.Carputer.Controls
{
    public class CustomMap : Map
    {
        public List<Position> RouteCoordinates { get; set; }

        public CustomMap()
        {
            RouteCoordinates = new List<Position>();
        }
    }
}
