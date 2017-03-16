using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
using Carputer.UWP.Messages;
using Carputer.UWP.Services;
using Carputer.UWP.ViewModels;

namespace Carputer.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : CaliburnApplication
    {
        private WinRTContainer _container;
        private IEventAggregator _eventAggregator;
        private IBootstrapService _bootstrapService;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            //this.Suspending += OnSuspending;

            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container
                .PerRequest<ShellViewModel>()
                .PerRequest<MainMenuViewModel>()
                .PerRequest<MapViewModel>(nameof(MapViewModel))
                .PerRequest<AutoViewModel>(nameof(AutoViewModel));

            _container.Singleton<IBootstrapService, BootstrapService>()
                .Singleton<ISettingsService, SettingsService>()
                .Singleton<IGPSService, GPSService>()
                .Singleton<ICacheService, CacheService>()
                .Singleton<IPowerShutdownMonitorService, PowerShutdownMonitorService>()
                .Singleton<IContinuousSpeechRecognizer, ContinuousSpeechRecognizer>();

            _eventAggregator = _container.GetInstance<IEventAggregator>();

            _bootstrapService = _container.GetInstance<IBootstrapService>();
            _bootstrapService.InitializeAsync();
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
            DisplayRootViewFor<ShellViewModel>();
            //DisplayRootViewFor<MapViewModel>();
            // It's kinda of weird having to use the event aggregator to pass 
            // a value to ShellViewModel, could be an argument for allowing
            // parameters or launch arguments

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                _eventAggregator.PublishOnUIThread(new ResumeStateMessage());
            }
        }

        protected async override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            _bootstrapService = _container.GetInstance<IBootstrapService>();
            await _bootstrapService.ShutdownAsync();

            _eventAggregator.PublishOnUIThread(new SuspendStateMessage(e.SuspendingOperation));
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
