using PropertyChanged;
using ST.Fx.Gps.NMEA;
using ST.Fx.Gps.NMEA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Carputer.ViewModels
{
    [ImplementPropertyChanged]
    public class NavigationPageModel : FreshMvvm.FreshBasePageModel
    {
        public double Latitude { get; set; } = 1.1;
        public double Longitude { get; set; } = 2.2;

        private IGpsService _gpsService = null;

        public NavigationPageModel(IGpsService gpsService)
        {
            _gpsService = gpsService;

            _gpsService.Positions.Subscribe(handler);
        }

        public override void Init(object initData)
        {
            base.Init(initData);
        }

        private void handler(GlobalPositioningSystemFixData message)
        {
            //Latitude = message.LatitudeDegrees;
            //Longitude = message.LongitudeDegrees;
        }
    }
}