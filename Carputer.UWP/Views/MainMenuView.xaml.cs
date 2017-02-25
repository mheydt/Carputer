using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Carputer.UWP.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Carputer.UWP.Views
{
    public sealed partial class MainMenuView : Page
    {
        public MainMenuView()
        {
            this.InitializeComponent();

            Loaded += MainMenuView_Loaded;
        }

        private void MainMenuView_Loaded(object sender, RoutedEventArgs e)
        {
        }
        /*
        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (DataContext as MainMenuViewModel).ItemClicked(e);
        }
        */
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
