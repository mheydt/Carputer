using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST.Apps.Carputer.ViewModels;
using Xamarin.Forms;

namespace ST.Apps.Carputer.Views
{
    public partial class NavigationPage : ContentPage
    {
        public NavigationPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var model = BindingContext as NavigationPageViewModel;
            model.PropertyChanged += Model_PropertyChanged;

            setCenter(model);
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => setCenter(sender as NavigationPageViewModel));
        }

        private void setCenter(NavigationPageViewModel model)
        {
            map.MoveToRegion(new Xamarin.Forms.Maps.MapSpan(new Xamarin.Forms.Maps.Position(model.Latitude, model.Longitude), 0.01, 0.01));
        }
    }
}
