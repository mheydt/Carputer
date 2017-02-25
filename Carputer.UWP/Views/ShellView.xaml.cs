using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Carputer.UWP.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carputer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : Page
    {
        public ShellView()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size() { Height = 480, Width = 800};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void ShellView_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            //(DataContext as ShellViewModel).SetupNavigationService(frame);
        }

        private void OpenNavigationView(object sender, RoutedEventArgs e)
        {
            NavigationView.IsPaneOpen = true;
        }
    }
}
