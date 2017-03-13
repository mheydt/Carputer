using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Carputer.UWP.Services;
using PropertyChanged;

namespace Carputer.UWP.ViewModels
{
    public class MainMenuViewModel : Screen
    {
        private INavigationService _navigationService;
        private IEventAggregator _eventAggregator;

        public string StartViewModelName { get; set; }

        public ObservableCollection<MainMenuItemViewModel> MainMenuItems { get; private set; }

        private MainMenuItemViewModel _selectedItem;
        public MainMenuItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                switch (_selectedItem.NavDestination)
                {
                    case "NavigationPage":
                        //_navigationService.For<MapViewModel>().Navigate();
                        _eventAggregator.PublishOnUIThread(new Messages.ActivateViewModelMessage<MapViewModel>());
                        break;

                    case "AutoPage":
                        _navigationService.For<AutoViewModel>().Navigate();
                        break;
                }
            }
        }

        public MainMenuViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            MainMenuItems = new ObservableCollection<MainMenuItemViewModel>()
            {
                new MainMenuItemViewModel("Auto",  "/Images/auto.png", "AutoPage"),
                new MainMenuItemViewModel("Music", "/Images/music.jpg", "MusicPage"),
                new MainMenuItemViewModel("Video", "/Images/video.png", "VideoPage"),
                new MainMenuItemViewModel("Nearby", "/Images/local.png", "LocalPage"),
                new MainMenuItemViewModel("Navigation", "/Images/nav.jpg", "NavigationPage"),
            };
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

         //   _navigationService.Navigate(typeof(MapViewModel));
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            //_navigationService.For<MapViewModel>().Navigate();
        }
    }
}
