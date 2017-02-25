using ST.Apps.Carputer.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ST.Apps.Carputer.Pages
{
    public partial class NavigationPage : ContentPage
    {
        public NavigationPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var model = BindingContext as NavigationPageModel;
            model.PropertyChanged += Model_PropertyChanged;

            setCenter(model);

            base.OnAppearing();
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => setCenter(sender as NavigationPageModel));
        }

        private void setCenter(NavigationPageModel model)
        {
            map.MoveToRegion(new Xamarin.Forms.Maps.MapSpan(new Xamarin.Forms.Maps.Position(model.Latitude, model.Longitude), 0.01, 0.01));
        }
    }
}
