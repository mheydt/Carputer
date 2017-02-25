using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST.Apps.Carputer.ViewModels;
using Xamarin.Forms;

namespace ST.Apps.Carputer.Views
{
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing() 
        {
            base.OnAppearing();
            /*
            lv.ItemsSource = 
                new ObservableCollection<MainMenuItemModel>()

            {
                new MainMenuItemModel("Auto",  "Images/auto.png"),
                new MainMenuItemModel("Music", "Images/music.jpg"),
                new MainMenuItemModel("Video", "Images/video.png"),
                new MainMenuItemModel("Nearby", "Images/local.png"),
                new MainMenuItemModel("Navigation", "Images/nav.jpg"),
            };
            '*/
        }
    }
}
