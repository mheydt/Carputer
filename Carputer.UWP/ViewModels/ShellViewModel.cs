using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Carputer.UWP.Messages;
using Carputer.UWP.Services;

namespace Carputer.UWP.ViewModels
{
    public class ShellViewModel : Screen, IHandle<ResumeStateMessage>, IHandle<SuspendStateMessage>
    {
        private readonly WinRTContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private INavigationService _navigationService;
        private bool _resume;
        private IBootstrapService _bootstrapService;
        private ISettingsService _settingsService;

        public ShellViewModel(WinRTContainer container, IEventAggregator eventAggregator, IBootstrapService bootstrapService, ISettingsService settingsService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _bootstrapService = bootstrapService;
            _settingsService = settingsService;
        }
        
        protected override void OnActivate()
        {
            _eventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
        }

        public async Task SetupNavigationService(Frame frame)
        {
            if (_container.HasHandler(typeof(INavigationService), null))
                _container.UnregisterHandler(typeof(INavigationService), null);

            _navigationService = _container.RegisterNavigationService(frame);

            if (_resume)
                _navigationService.ResumeState();

            var vm = await _settingsService.GetAsync<string>("StartViewModelName", "MapViewModel");

            _navigationService
                .For<MainMenuViewModel>()
                .WithParam(x => x.StartViewModelName, vm)
                .Navigate();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void Handle(SuspendStateMessage message)
        {
            _navigationService.SuspendState();
        }

        public void Handle(ResumeStateMessage message)
        {
            _resume = true;
        }


        public void GoBack()
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }

    }
}
