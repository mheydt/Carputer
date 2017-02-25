using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using ST.Apps.Carputer.Controls;
using ST.Apps.Carputer.UWP.Controls;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace ST.Apps.Carputer.UWP.Controls
{
    public class CustomMapRenderer : MapRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MapControl;
/*
                var coordinates = new List<BasicGeoposition>();
                foreach (var position in formsMap.RouteCoordinates)
                {
                    coordinates.Add(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude });
                }

                var polyline = new MapPolyline();
                polyline.StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0);
                polyline.StrokeThickness = 5;
                polyline.Path = new Geopath(coordinates);
                nativeMap.MapElements.Add(polyline);
                */
            }
        }
    }
}
