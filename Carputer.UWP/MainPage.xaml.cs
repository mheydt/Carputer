using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Carputer.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GestureRecognizer _gestures = new GestureRecognizer();

        public MainPage()
        {
            this.InitializeComponent();

            map.Center = new Geopoint(new BasicGeoposition() { Latitude = 46.935125833, Longitude = -114.07040899 });
            map.Style = MapStyle.AerialWithRoads;

            map.PointerMoved += Map_PointerMoved;
            map.PointerEntered += Map_PointerEntered;
            Loaded += MainPage_Loaded;

            //grid.ManipulationStarted += OnManipulationStarted;
            //grid.ManipulationCompleted += OnManipulationCompleted;
            //grid.ManipulationMode = ManipulationModes.All;

            //overlay.PointerMoved += OverlayOnPointerMoved;

        }

        private void Map_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("Map_PointerEntered");
        }

        private void Map_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("Map_PointerMoved");
        }

        private void OverlayOnPointerMoved(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
//            pointerRoutedEventArgs.Handled = true;
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs manipulationStartedRoutedEventArgs)
        {
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs manipulationCompletedRoutedEventArgs)
        {
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //PointerMoved += MainPage_PointerMoved;

            //_gestures.CrossSliding += _gestures_CrossSliding;
            //_gestures.GestureSettings = GestureSettings.CrossSlide;

            _gestures.ManipulationStarted += _gestures_ManipulationStarted;
            _gestures.ManipulationCompleted += _gestures_ManipulationCompleted;

            grid.PointerMoved += Grid_PointerMoved;
            grid.PointerCanceled += Grid_PointerCanceled;
            grid.PointerPressed += Grid_PointerPressed;
            grid.PointerReleased += Grid_PointerReleased;

            gLeft.PointerEntered += GLeft_PointerEntered;

            /*
            var edgeGestures = Windows.UI.Input.EdgeGesture.GetForCurrentView();
            edgeGestures.Starting += EdgeGestures_Starting;
            edgeGestures.Completed += EdgeGestures_Completed;
            */
        }

        private void GLeft_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("gLeft_PointerEntered");
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Grid_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Grid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("Grid_PointerMoved");
            _gestures.ProcessMoveEvents(e.GetIntermediatePoints(grid));
            e.Handled = true;
        }

        private void _gestures_ManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            Debug.WriteLine($"Manipulation completed");
        }

        private void _gestures_ManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {
            Debug.WriteLine($"Manipulation started");
        }

        private void EdgeGestures_Completed(EdgeGesture sender, EdgeGestureEventArgs args)
        {
            Debug.WriteLine($"Edge gesture completed: {args.Kind}");
        }

        private void EdgeGestures_Starting(EdgeGesture sender, EdgeGestureEventArgs args)
        {
            Debug.WriteLine($"Edge gesture started: {args.Kind}");
        }

        private void MainPage_PointerMoved(object sender, PointerRoutedEventArgs args)
        {
            var p = args.GetCurrentPoint(grid);
            Debug.WriteLine($"Pointer Moved: {p.Position.X} {p.Position.Y}");

            _gestures.ProcessMoveEvents(args.GetIntermediatePoints(this.grid));

            args.Handled = true;
        }

        private void _gestures_CrossSliding(GestureRecognizer sender, CrossSlidingEventArgs args)
        {
            Debug.WriteLine($"Cross sliding: {args.CrossSlidingState} {args.Position.X} {args.Position.Y}");
        }

        private void Grid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("Grid_OnTapped");
        }
    }
}
