using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ST.Apps.Carputer.Views
{
    public partial class FooPage : ContentPage
    {
        public FooPage()
        {
            InitializeComponent();

            Content = new Image()
            {
                Source =// "https://developer.xamarin.com/demo/IMG_3256.JPG"
                ImageSource.FromFile("images/auto.png")
            };
        }
    }
}
