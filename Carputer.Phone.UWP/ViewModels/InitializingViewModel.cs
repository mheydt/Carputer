using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Carputer.Phone.UWP.Models;

namespace Carputer.Phone.UWP.ViewModels
{
    public class InitializingViewModel : Screen
    {
        private IAppModel _appModel;

        public InitializingViewModel(IAppModel appModel)
        {
            _appModel = appModel;
        }

        protected override async void OnInitialize()
        {
            await _appModel.InitializeAsync();
        }
    }
}
