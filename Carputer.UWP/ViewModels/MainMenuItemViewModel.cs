using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using PropertyChanged;

namespace Carputer.UWP.ViewModels
{
    [ImplementPropertyChanged]
    public class MainMenuItemViewModel
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

        public MainMenuItemViewModel(string title, string imagePath, string navDestination)
        {
            Title = title;
            ImagePath = imagePath;
            NavDestination = navDestination;
        }
    }
}
