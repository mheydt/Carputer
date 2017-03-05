using ST.Fx.OBDII.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Carputer.Phone.UWP.OBDII
{
    public class SimulatedClient : IOBDIITransport
    {
        public async Task<string> ExecuteCommand(string command, string terminator = ">", CancellationToken cancellation = default(CancellationToken))
        {
            return await Task.FromResult(ObdUtils.GetEmulatorValue(command));
        }

        public async Task<bool> InitAsync(CancellationToken cancellation = default(CancellationToken))
        {
            return await Task.FromResult(true);
        }

        public async Task ShutdownAsync(CancellationToken cancellation = default(CancellationToken))
        {
            await Task.CompletedTask;
        }
    }
}
