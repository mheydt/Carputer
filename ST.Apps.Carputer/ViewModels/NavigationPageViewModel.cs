using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;

namespace ST.Apps.Carputer.ViewModels
{
    [ImplementPropertyChanged]
    public class NavigationPageViewModel : BindableBase, INavigationAware
    {
        public double Latitude { get; set; } = 46.935125833;
        public double Longitude { get; set; } = -114.07040899;
        public double LatDegrees { get; set; } = 0.01;
        public double LongDegrees { get; set; } = 0.01;

        public NavigationPageViewModel()
        {
            
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
