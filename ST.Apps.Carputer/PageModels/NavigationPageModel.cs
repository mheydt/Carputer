using FreshMvvm;
using PropertyChanged;
using ST.Fx.Gps.NMEA;
using ST.Fx.Gps.NMEA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Apps.Carputer.PageModels
{
    [ImplementPropertyChanged]
    public class NavigationPageModel : FreshBasePageModel
    {
        private IGpsService _gpsService;

        public double Latitude { get; set; }// = 46.935125833;
        public double Longitude { get; set; }// = -114.07040899;

        public NavigationPageModel(IGpsService gpsService)
        {
            _gpsService = gpsService;

            _gpsService.Positions.Subscribe(positionUpdate);
        }

        private void positionUpdate(GlobalPositioningSystemFixData update)
        {
            if (Latitude != update.LatitudeDegrees)
            {
                Latitude = update.LatitudeDegrees;
            }
            if (Longitude != update.LongitudeDegrees)
            {
                Longitude = update.LongitudeDegrees;
            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }
    }
}
