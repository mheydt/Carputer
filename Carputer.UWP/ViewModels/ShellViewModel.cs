using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Carputer.UWP.Messages;
using Carputer.UWP.Services;
using System.Windows.Input;
using Carputer.UWP.Views;

namespace Carputer.UWP.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<ResumeStateMessage>, IHandle<SuspendStateMessage>, IHandle<ActivateViewModelMessage>
    {
        private readonly WinRTContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private INavigationService _navigationService;
        private bool _resume;
        private IBootstrapService _bootstrapService;
        private ISettingsService _settingsService;

        private List<Screen> _viewModelStack = new List<Screen>();

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

            navigate(typeof(MainMenuViewModel));
        }

        private void navigate(Type viewModelType)
        { 
            var vm = _container.GetAllInstances(viewModelType).First();
            ActivateItem(vm);
///            ActivateItem(new MainMenuViewModel(_navigationService));
        }


        private void navigate(Screen viewModel)
        {
            if (ActiveItem != null) _viewModelStack.Add((Screen)ActiveItem);
            ActivateItem(viewModel);
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

            /*
            _navigationService
                .For<MainMenuViewModel>()
                .WithParam(x => x.StartViewModelName, vm)
                .Navigate();
                */

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


        public void GoBack(object context)
        {
            var v = GetView() as ShellView;
            if (v != null) v.closePanel();

            if (_viewModelStack.Count > 0)
            {
                var vm = _viewModelStack.Last();
                _viewModelStack.Remove(vm);
                navigate(vm);
            }
        }

        public void Handle(ActivateViewModelMessage message)
        {
            var vm = _container.GetAllInstances(message.ViewModelType).First();
            navigate((Screen)vm);
        }
    }
}
