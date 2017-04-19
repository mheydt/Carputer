using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Carputer.Phone.UWP.SMAC
{
    public class MessageFacade
    {
        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task ShutdownAsync()
        {
            await Task.CompletedTask;
        }

        public void Send(object message)
        {
            var json = JsonConvert.SerializeObject(message);
            var env = new ServiceMessageEnvelope();
            env.DataTypeName = message.GetType().FullName;
            env.Data = json;
        }
    }
}
