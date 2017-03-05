using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public class NullOBDIIServer : IOBDIIServer
    {
        public async Task InitAsync(CancellationToken cancellation = default(CancellationToken))
        {
            await Task.CompletedTask;
        }

        public async Task ShutdownAsync(CancellationToken cancellation = default(CancellationToken))
        {
            await Task.CompletedTask;
        }

        public async Task<string> ExecuteCommand(string command, CancellationToken cancellation = default(CancellationToken))
        {
            return await Task.FromResult("");
        }
    }
}
