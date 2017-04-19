using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Carputer.Phone.UWP
{
    public abstract class BackgroundServiceTaskHelper
    {
        public static bool register<T>(string taskName) where T : IBackgroundTask
        {
            var tasks = BackgroundTaskRegistration.AllTasks.ToArray();
            if (tasks.Any(t => t.Value.Name == taskName)) return true;

            try
            {
                var socketTaskBuilder = new BackgroundTaskBuilder();
                socketTaskBuilder.Name = taskName;
                socketTaskBuilder.TaskEntryPoint = typeof(T).FullName;
                var trigger = new SocketActivityTrigger();
                socketTaskBuilder.SetTrigger(trigger);
                var registration = socketTaskBuilder.Register();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
