﻿using PropertyChanged;
using ST.Fx.Debug.Tracer;
using ST.Fx.OBDII;
using ST.Fx.OBDII;
using ST.Fx.OBDII.Wifi.PCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Carputer.Phone.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private OBDIIService _obd2 = new OBDIIService(new TCPStreamingTransport("192.168.0.10", 35000));

        [ImplementPropertyChanged]
        public class Model
        {
            public ST.Fx.OBDII.Model OBDII { get; set; }
            public ObservableCollection<string> Traces { get; set; } = new ObservableCollection<string>();
            public string RPM { get; set; }
        }

        private Model _model = new Model();
        private DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Tracer.removeListener(trace);
            _timer.Stop();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Tracer.addListener(trace);

            await _obd2.InitAsync(true);

            DataContext = _model;

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(0.25)
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();
            getData();
        }

        private void _timer_Tick(object sender, object e)
        {
            getData();
            //trace($"tick {_count++}");
        }

        private int _count = 0;
        private void getData()
        {
            var readings = _obd2.GetState();
            if (readings != null &&readings.ContainsKey("rpm"))
            {
                _model.RPM = readings["rpm"];
                rpm.Value = double.Parse(_model.RPM);
            }
            else
            {
                _model.RPM = "";
            }
        }

        private void trace(string msg)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                _model.Traces.Insert(0, msg);
            }));
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            await _obd2.DisconnectAsync();
            await _obd2.InitAsync();
        }

        private async void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            await _obd2.DisconnectAsync();
        }
    }
}