using ST.Apps.Carputer.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Unity;
using ST.Apps.Carputer.Views;
using Xamarin.Forms;

namespace ST.Apps.Carputer
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
            
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("NavigationPage/MainMenuPage"); // MainMenuPage?title=Hello%20from%20Xamarin.Forms");
            //NavigationService.NavigateAsync("FooPage"); // MainMenuPage?title=Hello%20from%20Xamarin.Forms");
            //MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<NavigationPageModel>();
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<Xamarin.Forms.NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<MainMenuPage>();
            Container.RegisterTypeForNavigation<FooPage>();
            Container.RegisterTypeForNavigation<ST.Apps.Carputer.Views.NavigationPage>();
        }
    }
}
