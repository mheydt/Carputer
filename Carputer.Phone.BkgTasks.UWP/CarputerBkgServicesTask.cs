using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Carputer.Phone.BkgTasks.UWP
{
    public sealed class CarputerBkgServicesTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        public CarputerBkgServicesTask()
        {
            
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            _deferral.Complete();
        }
    }
}
