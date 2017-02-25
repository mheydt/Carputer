using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;
using Xamarin.Forms;

namespace ST.Apps.Carputer.ViewModels
{
    public class MainMenuItemModel : BindableBase
    {
        public string Title { get; set; }
        public string ImagePath { get; set; }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get
            {
                return _imageSource;
            }

            set { _imageSource = value; }
        }

        public string NavDestination { get; set; }

        public MainMenuItemModel(string title, string imagePath, string navDestination)
        {
            Title = title;
            ImagePath = imagePath;
            _imageSource = ImageSource.FromFile(imagePath);
            NavDestination = navDestination;
        }
    }

    public class MainMenuPageViewModel : BindableBase, INavigationAware
    {
        public int Count {  get { return MainMenuItems.Count; } }

        private ObservableCollection<MainMenuItemModel> _mainMenuItems;
        public ObservableCollection<MainMenuItemModel> MainMenuItems
        {
            get { return _mainMenuItems; }
            set
            {
                _mainMenuItems = value; 
            }
        }

        private INavigationService _navigationService;
        public MainMenuPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            MainMenuItems = new ObservableCollection<MainMenuItemModel>()
            {
                new MainMenuItemModel("Auto",  "Images/auto.png", "AutoPage"),
                new MainMenuItemModel("Music", "Images/music.jpg", "MusicPage"),
                new MainMenuItemModel("Video", "Images/video.png", "VideoPage"),
                new MainMenuItemModel("Nearby", "Images/local.png", "LocalPage"),
                new MainMenuItemModel("Navigation", "Images/nav.jpg", "NavigationPage"),
            };
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
