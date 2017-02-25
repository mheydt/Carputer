using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ST.Carputer.UWP
{
    public sealed partial class MainPage : WindowsPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new ST.Carputer.App());
        }
    }
}
