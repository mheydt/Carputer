using Carputer.Services;
using ST.Fx.OBDII;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TinyIoC;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Carputer.Phone.UWP.Models;
using Carputer.Phone.UWP.Services;
using Carputer.Phone.UWP.ViewModels;

namespace Carputer.Phone.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : CaliburnApplication
    {
        private WinRTContainer _container;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container.PerRequest<ShellViewModel>()
                .PerRequest<InitializingViewModel>()
                .PerRequest<HomeViewModel>();

            _container
                .Singleton<IAppModel, AppModel>()
                .Singleton<ICarputerProxyService, CarputerProxyService>()
                .Singleton<ILocationService, LocationService>();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            // Note we're using DisplayRootViewFor (which is view model first)
            // this means we're not creating a root frame and just directly
            // inserting ShellView as the Window.Content

            //DisplayRootViewFor<ShellViewModel>();
            //DisplayRootViewFor<ShellViewModel>();
            //DisplayRootViewFor<MapViewModel>();

            DisplayRootViewFor<ShellViewModel>();


            // It's kinda of weird having to use the event aggregator to pass 
            // a value to ShellViewModel, could be an argument for allowing
            // parameters or launch arguments

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //_eventAggregator.PublishOnUIThread(new ResumeStateMessage());
            }
            /*
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            //if (rootFrame == null)
            //{
                /*
                var deviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
                var numDevices = deviceInfoCollection.Count();
                DeviceInformation device = null;
                foreach (var info in deviceInfoCollection)
                {
                    //if (info.Name.ToLower().Contains("obd"))
                    {
                        device = info;
                    }
                }
                //if (device == null) return false;

                //try
                //{
                var _service = await RfcommDeviceService.FromIdAsync(device.Id);
                    //}
                    */
            /*
        TinyIoCContainer.Current.Register<IAppServices, AppServices>().AsSingleton();
        TinyIoCContainer.Current.Register<IOBDIIService, OBDIIService>().AsSingleton();
        TinyIoCContainer.Current.Register<IOBDIIDataProcessor, OBDIIDataProcessor>().AsSingleton();
        //TinyIoCContainer.Current.Register<IOBDIITransport, SocketClientTransport>().AsSingleton();
        TinyIoCContainer.Current.Register<IOBDIITransport, BluetoothClient>().AsSingleton();
        TinyIoCContainer.Current.Register<IOBDIIServer, NullOBDIIServer>().AsSingleton();

        var services = TinyIoCContainer.Current.Resolve<IAppServices>();
        await services.InitializeAsync();

        // Create a Frame to act as the navigation context and navigate to the first page
        rootFrame = new Frame();

        rootFrame.NavigationFailed += OnNavigationFailed;

        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
        {
            //TODO: Load state from previously suspended application
        }

        // Place the frame in the current Window
        Window.Current.Content = rootFrame;

    }

    if (e.PrelaunchActivated == false)
    {
        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), e.Arguments);
        }
        // Ensure the current window is active
        Window.Current.Activate();
    }
    */
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
